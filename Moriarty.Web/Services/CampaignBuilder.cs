using System.Text;
using System.Text.Json;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.TextToImage;
using Moriarty.Web.Data.Models;

namespace Moriarty.Web.Services;

public class CampaignBuilder
{
    private readonly Kernel _kernel;
    private readonly PromptLoader _promptLoader;

    private readonly ILogger<CampaignBuilder> _logger;

    public CampaignBuilder(Kernel kernel, PromptLoader promptLoader, ILoggerFactory loggerFactory)
    {
        _kernel = kernel;
        _promptLoader = promptLoader;
        _logger = loggerFactory.CreateLogger<CampaignBuilder>();
    }
    
    public async Task<Campaign> GenerateDraftAsnyc(
        string background,
        int suspects,
        string language,
        CancellationToken cancellationToken
    ) {
        string prompt = _promptLoader.Load("GenerateCampaign.yaml");
        KernelFunction function = _kernel.CreateFunctionFromPromptYaml(prompt);
        string asJson = await _kernel.InvokeAsync<string>(
            function,
            arguments: new()
            {
                { "background", background },
                { "language", language },
                { "suspects", suspects },
            },
            cancellationToken: cancellationToken
        );

        return JsonSerializer.Deserialize<Campaign>(asJson);
    }

    public async Task<Clue> CreateClueAsync(Campaign campaign, CancellationToken cancellationToken)
    {
        string prompt = _promptLoader.Load("CreateClue.yaml");
        KernelFunction function = _kernel.CreateFunctionFromPromptYaml(prompt);
        string asJson = await _kernel.InvokeAsync<string>(
            function,
            arguments: new()
            {
                { "campaign", JsonSerializer.Serialize(campaign) },
            },
            cancellationToken);

        return JsonSerializer.Deserialize<Clue>(asJson);
    }

    public async Task<string> EnhanceCharacterDescriptionAsync(
        Campaign campaign,
        Character character,
        CancellationToken cancellationToken
    )
    {
        string prompt = _promptLoader.Load("EnhanceCharacterDescription.yaml");
        KernelFunction function = _kernel.CreateFunctionFromPromptYaml(prompt);
        return await _kernel.InvokeAsync<string>(
            function,
            arguments: new()
            {
                { "campaign", JsonSerializer.Serialize(campaign) },
                { "character", JsonSerializer.Serialize(character) },
            },
            cancellationToken);
    }

    public async Task<string> UploadFileAsync(
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
}