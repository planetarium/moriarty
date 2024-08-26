using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Moriarty.Web.Components.Pages;

namespace Moriarty.Web.Services;

public class SceneDirector(Kernel kernel, PromptLoader promptLoader, ILoggerFactory loggerFactory)
{
    private readonly ILogger<SceneDirector> _logger = loggerFactory.CreateLogger<SceneDirector>();

    public async Task InvokeAsync(
        Guid campaignId,
        ChatHistory chatHistory,
        string currentScene,
        CancellationToken cancellationToken)
    {
        string systemPrompt = promptLoader.Load("SceneDirector.txt");
        string functionPrompt = promptLoader.Load("DirectScene.txt");
        KernelFunction function = kernel.CreateFunctionFromPrompt(
            functionPrompt,
            executionSettings: new OpenAIPromptExecutionSettings()
            {
                ModelId = "gpt-4o",
                ChatSystemPrompt = systemPrompt,
                ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions,
            });
        _ = await kernel.InvokeAsync(function, new()
        {
            { "chat_history", string.Join("\n", chatHistory) },
            { "campaign_id", campaignId.ToString() },
            { "current_scene", currentScene },
        }, cancellationToken);
    }
}