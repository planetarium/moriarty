using Microsoft.Extensions.Configuration;
using Moriarty.Web.Services;
using Xunit.Abstractions;

namespace Moriarty.Tests;

public class SemanticKernelServiceTest
{
    private readonly ITestOutputHelper _output;
    private readonly IConfiguration _configuration;

    public SemanticKernelServiceTest(ITestOutputHelper output)
    {
        _output = output;
        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                ["OpenAI:ChatCompletionModel"] = "gpt-4o",
            })
            .AddUserSecrets<SemanticKernelService>().Build();
    }

    [Fact]
    public async Task TextToImage()
    {
        var service = new SemanticKernelService(_configuration, new PromptLoader());
        var response = await service.GeneratePotraitAsync(
            background: "Cyberfunk",
            plot: "미래의 도시 네오 서울에서, 경찰 탐정 김현수는 한 기업 총수의 의문의 죽음을 조사하게 된다. 이 복잡한 사건은 사이버펑크 배경 속에서 펼쳐지며, 다양한 용의자들과 그들만의 비밀이 얽혀 있다. 김현수는 네온 불빛 속에서 진실을 밝혀내기 위해 고군분투한다.",
            name: "한재영",
            description: "네오 서울의 대기업 총수로, 여러 사람과 갈등을 빚어왔다.",
            age: 55,
            cancellationToken: default
        );
        _output.WriteLine("image: " + response);
    }
}