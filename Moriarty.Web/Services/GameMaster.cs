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
    
    public IAsyncEnumerable<StreamingChatMessageContent> ChatAsync(
        Guid campaignId,
        ChatHistory history,
        string language,
        CancellationToken cancellationToken
    )
    {
        string systemPrompt = _promptLoader.Load("GameMaster.txt");
        IChatCompletionService completion = _kernel.GetRequiredService<IChatCompletionService>();
        systemPrompt += $"Current Campaign is {campaignId}\n";
        systemPrompt += $"You should answer in {language}\n";
        return completion.GetStreamingChatMessageContentsAsync(
            history,
            executionSettings: new OpenAIPromptExecutionSettings()
            {
                ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions,
                ChatSystemPrompt = systemPrompt,
                Temperature = 0.7,
                TopP = 0.7,
                StopSequences = ["Player: "],
            },
            kernel: _kernel,
            cancellationToken: cancellationToken);
    }
    
    public async Task<List<string>> SuggestNextPromptAsync(
        ChatHistory history,
        CancellationToken cancellationToken)
    {
        string prompt = _promptLoader.Load("SuggestNextPrompt.yaml");
        KernelFunction function = _kernel.CreateFunctionFromPromptYaml(prompt);
        string llmResponse = await _kernel.InvokeAsync<string>(
            function,
            arguments: new()
            {
                {"chat_history", string.Join("\n", history.Select(c => c.Content)) }
            },
            cancellationToken);
        
        // FIXME remove assuming "prompts". 
        return JsonSerializer.Deserialize<Dictionary<string, List<string>>>(llmResponse)["prompts"];
    }
}