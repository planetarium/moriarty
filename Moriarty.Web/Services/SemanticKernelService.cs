using System.Text;
using System.Text.Json;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Memory;
using Microsoft.SemanticKernel.Plugins.Memory;
using Microsoft.SemanticKernel.TextToImage;
using Moriarty.Web.Data.Models;
using Moriarty.Web.LLMPlugins;

namespace Moriarty.Web.Services;

public class SemanticKernelService {
    private readonly Kernel _kernel;
    private readonly PromptLoader _promptLoader;

    public SemanticKernelService(
        IConfiguration configuration,
        PromptLoader promptLoader,
        CampaignPlugin campaignPlugin,
        ILoggerFactory loggerFactory) {
        string apiKey = configuration["OpenAI:ApiKey"];
        string chatCompletionModel = configuration["OpenAI:ChatCompletionModel"];
        string textToImageModel = configuration["OpenAI:TextToImageModel"];

        IKernelBuilder builder = Kernel.CreateBuilder()
            .AddOpenAIChatCompletion(modelId: chatCompletionModel, apiKey: apiKey)
            .AddOpenAITextToImage(modelId: textToImageModel, apiKey: apiKey)
            .AddOpenAIFiles(apiKey);

        builder.Services.AddSingleton(loggerFactory);
        builder.Plugins.AddFromObject(campaignPlugin);
        _kernel = builder.Build();
        _promptLoader = promptLoader;
    }

    public Task<string> GenerateCampaign(
        string background,
        int suspects,
        string language,
        CancellationToken cancellationToken
    ) {
        string prompt = _promptLoader.Load("GenerateCampaign.yaml");
        KernelFunction function = _kernel.CreateFunctionFromPromptYaml(prompt);
        return _kernel.InvokeAsync<string>(
            function,
            arguments: new()
            {
                { "background", background },
                { "language", language },
                { "suspects", suspects },
            },
            cancellationToken: cancellationToken
        );
    }

    public async Task<string> UploadFile(
        Campaign campaign,
        CancellationToken cancellationToken)
    {
        OpenAIFileService fileService = _kernel.GetRequiredService<OpenAIFileService>();
        ReadOnlyMemory<byte> bytes = new(
            Encoding.UTF8.GetBytes(campaign.LLMInstruction)
        );
        OpenAIFileReference fileRef = await fileService.UploadContentAsync(
            new BinaryContent(bytes, "text/plain"),
            new OpenAIFileUploadExecutionSettings(
                $"campaign-{campaign.Title}-{campaign.GetHashCode()}.txt",
                OpenAIFilePurpose.Assistants
            ),
            cancellationToken: cancellationToken
        );
        return fileRef.Id;
    }

    public async Task<string> Chat(
        Guid campaignId,
        ChatHistory history,
        CancellationToken cancellationToken
    )
    {
        string gmPrompt = _promptLoader.Load("GameMaster.txt");
        IChatCompletionService completion = _kernel.GetRequiredService<IChatCompletionService>();
        var chatSystemPrompt = gmPrompt + $"Current Campaign is {campaignId}";
        var content = await completion.GetChatMessageContentsAsync(
            history,
            executionSettings: new OpenAIPromptExecutionSettings()
            {
              ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions,
              ChatSystemPrompt = chatSystemPrompt,
              Temperature = 0.7,
              TopP = 0.7,
            },
            kernel: _kernel,
            cancellationToken: cancellationToken);
        return content[0].Content;
    }

    public async Task<string> GeneratePortraitAsync(
        string background,
        string plot,
        string name,
        string description,
        int age,
        CancellationToken cancellationToken
    ) {
        string prompt = _promptLoader.Load("GeneratePortrait.yaml");
        ITextToImageService tti = _kernel.GetRequiredService<ITextToImageService>();
        KernelFunction function = _kernel.CreateFunctionFromPromptYaml(prompt);
        string ttiPrompt = await _kernel.InvokeAsync<string>(
            function,
            arguments: new()
            {
                { "background", background },
                { "plot", plot },
                { "name", name },
                { "description", description },
                { "age", age },
            },
            cancellationToken: cancellationToken
        );

        return await tti.GenerateImageAsync(
            ttiPrompt,
            1024,
            1024,
            cancellationToken: cancellationToken
        );
    }

    public async Task<List<string>> SuggestNextPrompt(
        string history,
        CancellationToken cancellationToken)
    {
        string prompt = _promptLoader.Load("SuggestNextPrompt.yaml");
        KernelFunction function = _kernel.CreateFunctionFromPromptYaml(prompt);
        string llmResponse = await _kernel.InvokeAsync<string>(
            function,
            arguments: new()
            {
                {"chat_history", history }
            },
            cancellationToken);
        
        // FIXME remove assuming "prompts". 
        return JsonSerializer.Deserialize<Dictionary<string, List<string>>>(llmResponse)["prompts"];
    }
}