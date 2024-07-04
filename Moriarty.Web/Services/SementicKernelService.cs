using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.TextToImage;

namespace Moriarty.Web.Services;

public class SementicKernelService {
    private readonly Kernel _kernel;

    private readonly PromptLoader _promptLoader;

    public SementicKernelService(IConfiguration configuration, PromptLoader promptLoader) {
        string apiKey = configuration["OpenAI:ApiKey"];
        string model = configuration["OpenAI:ChatCompletionModel"];
        var builder = Kernel.CreateBuilder();
        builder.AddOpenAIChatCompletion(model, apiKey);
        builder.AddOpenAITextToImage(apiKey);

        _kernel = builder.Build();
        _promptLoader = promptLoader;
    }

    public Task<string> GetCompletion(string prompt, CancellationToken cancellationToken) {
        return _kernel.InvokePromptAsync<string>(prompt, cancellationToken: cancellationToken);
    }

    public Task<string> GenerateCampaign(CancellationToken cancellationToken) {
        string prompt = _promptLoader.Load("GenerateCampaign");
        KernelFunction function = _kernel.CreateFunctionFromPromptYaml(prompt);
        return _kernel.InvokeAsync<string>(
            function,
            arguments: new()
            {
                { "language", "Korean" },
            },
            cancellationToken: cancellationToken
        );
    }

    public Task<string> GenerateImage(string prompt, CancellationToken cancellationToken) {
        var dallE = _kernel.GetRequiredService<ITextToImageService>();
        return dallE.GenerateImageAsync(
            prompt,
            512,
            512,
            _kernel,
            cancellationToken: cancellationToken
        );
    }
}