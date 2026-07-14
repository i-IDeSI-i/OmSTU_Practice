using System.Reflection;
using PluginContracts;

namespace PluginHost;

public static class PluginLoader
{
    public static IReadOnlyList<PluginInfo> Discover(string directory)
    {
        return Directory
            .GetFiles(directory, "*.dll")
            .SelectMany(SafeLoadTypes)
            .Select(t => new { Type = t, Attr = t.GetCustomAttribute<PluginLoadAttribute>() })
            .Where(x => x.Attr is not null)
            .Select(x => new PluginInfo
            {
                Name = x.Attr!.Name,
                DependsOn = x.Attr.DependsOn,
                Type = x.Type
            })
            .ToList();
    }

    private static IEnumerable<Type> SafeLoadTypes(string dllPath)
    {
        try
        {
            return Assembly.LoadFrom(dllPath).GetTypes();
        }
        catch
        {
            return Enumerable.Empty<Type>();
        }
    }
}