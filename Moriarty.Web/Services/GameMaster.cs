using System.Text.Json;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace Moriarty.Web.Services;

public class GameMaster
{
    private readonly Kernel _kernel;
    private readonly PromptLoader _promptLoader;
    private readonly ILogger<GameMaster> _logger;

    public GameMaster(Kernel kernel, PromptLoader promptLoader, ILoggerFactory loggerFactory)
    {
        _kernel = kernel;
        _promptLoader = promptLoader;
        _logger = loggerFactory.CreateLogger<GameMaster>();
    }
    
    public async Task<string> ChatAsync(
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
    
    public async Task<List<string>> SuggestNextPromptAsync(
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