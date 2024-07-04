using Microsoft.Extensions.Configuration;
using Moriarty.Web.Services;
using Xunit.Abstractions;

namespace Moriarty.Tests;

public class SementicKernelServiceTest
{
    private readonly ITestOutputHelper _output;
    private readonly IConfiguration _configuration;

    public SementicKernelServiceTest(ITestOutputHelper output)
    {
        _output = output;
        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                ["OpenAI:ChatCompletionModel"] = "gpt-4o",
            })
            .AddUserSecrets<SementicKernelService>().Build();
    }

    [Fact]
    public async Task GenerateCampaign()
    {
        var service = new SementicKernelService(_configuration, new PromptLoader());
        var response = await service.GenerateCampaign(default);
        _output.WriteLine(response);
    }

    [Fact]
    public async Task TextToImage()
    {
        var service = new SementicKernelService(_configuration, new PromptLoader());
        var response = await service.GenerateImage("White rabbit jumping on the grass, cartoon style", default);
        _output.WriteLine(response);
    }
}