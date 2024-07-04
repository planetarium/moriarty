using Moriarty.Web.Services;
using Xunit;

public class PromptLoaderTest 
{
    [Fact]
    public void Load() 
    {
        var loader = new PromptLoader();
        Assert.Throws<ArgumentException>(() => { loader.Load("xxx"); });
    }
}