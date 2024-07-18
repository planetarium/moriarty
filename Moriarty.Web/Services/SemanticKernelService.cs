using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.TextToImage;

namespace Moriarty.Web.Services;

public class SemanticKernelService {
    private readonly Kernel _kernel;
    private readonly PromptLoader _promptLoader;

    public SemanticKernelService(IConfiguration configuration, PromptLoader promptLoader) {
        string apiKey = configuration["OpenAI:ApiKey"];
        string model = configuration["OpenAI:ChatCompletionModel"];
        var builder = Kernel.CreateBuilder();
        builder.AddOpenAIChatCompletion(model, apiKey);
        builder.AddOpenAITextToImage(modelId: "dall-e-3", apiKey: apiKey);

        _kernel = builder.Build();
        _promptLoader = promptLoader;
    }

    public Task<string> GenerateCampaign(
        string background,
        int suspects,
        string language,
        CancellationToken cancellationToken
    ) {
        string prompt = _promptLoader.Load("GenerateCampaign");
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

    public async Task<string> GeneratePotraitAsync(
        string background,
        string plot,
        string name,
        string description,
        int age,
        CancellationToken cancellationToken
    ) {
        string prompt = _promptLoader.Load("GeneratePotrait");
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
}