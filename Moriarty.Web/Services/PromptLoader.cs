using System.Reflection;

namespace Moriarty.Web.Services;

public class PromptLoader 
{
    private readonly Assembly _assembly;

    public PromptLoader()
    {
        _assembly = Assembly.GetAssembly(typeof(PromptLoader));
    }

    public string Load(string name)
    {
        using var stream = _assembly.GetManifestResourceStream($"{_assembly.GetName().Name}.Embedded.{name}.yaml")
            ?? throw new ArgumentException($"Given resource name {name} doesn't exist.", nameof(name));
        return new StreamReader(stream).ReadToEnd();
    }
}