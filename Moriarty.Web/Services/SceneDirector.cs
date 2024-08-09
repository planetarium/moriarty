using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Moriarty.Web.Components.Pages;

namespace Moriarty.Web.Services;

public class SceneDirector(Kernel kernel, PromptLoader promptLoader, ILoggerFactory loggerFactory)
{
    private readonly ILogger<SceneDirector> _logger = loggerFactory.CreateLogger<SceneDirector>();

    public async Task InvokeAsync(Guid campaignId, ChatHistory chatHistory, CancellationToken cancellationToken)
    {
        string systemPrompt = promptLoader.Load("SceneDirector.txt");
        systemPrompt += $"\n Current Campaign is {campaignId}";
        KernelFunction function = kernel.CreateFunctionFromPrompt(
            "Describe the place and display the character card where the conversation takes place. Chat History: {{$chatHistory}}",
            executionSettings: new OpenAIPromptExecutionSettings()
            {
                ModelId = "gpt-4o",
                ChatSystemPrompt = systemPrompt,
                ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions,
            });
        var res = await kernel.InvokeAsync(function, new()
        {
            { "chatHistory", string.Join("\n", chatHistory) },
        }, cancellationToken);
    }
}