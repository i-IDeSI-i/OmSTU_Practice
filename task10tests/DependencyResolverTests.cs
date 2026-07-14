using Xunit;
using PluginHost;

public class DependencyResolverTests
{
    private static PluginInfo Plugin(string name, params string[] deps)
        => new() { Name = name, DependsOn = deps, Type = typeof(object) };

    [Fact]
    public void Sort_OrdersDependenciesBeforeDependents()
    {
        var plugins = new List<PluginInfo>
        {
            Plugin("Cache", "Database"),
            Plugin("Database", "Logger"),
            Plugin("Logger")
        };

        var result = DependencyResolver.Sort(plugins).Select(p => p.Name).ToList();

        Assert.True(result.IndexOf("Logger") < result.IndexOf("Database"));
        Assert.True(result.IndexOf("Database") < result.IndexOf("Cache"));
    }

    [Fact]
    public void Sort_IndependentPlugins_AllPresent()
    {
        var plugins = new List<PluginInfo>
        {
            Plugin("A"),
            Plugin("B"),
            Plugin("C")
        };

        var result = DependencyResolver.Sort(plugins);
        Assert.Equal(3, result.Count);
    }

    [Fact]
    public void Sort_CyclicDependency_Throws()
    {
        var plugins = new List<PluginInfo>
        {
            Plugin("X", "Y"),
            Plugin("Y", "X")
        };

        Assert.Throws<InvalidOperationException>(() => DependencyResolver.Sort(plugins));
    }
}