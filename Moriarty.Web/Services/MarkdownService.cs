using Markdig;
namespace Moriarty.Web.Services;

public class MarkdownService
{
    public string RenderMarkdown(string markdown)
    {
        var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
        return Markdown.ToHtml(markdown, pipeline);
    }
}