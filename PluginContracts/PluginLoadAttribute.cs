namespace PluginContracts;

[AttributeUsage(AttributeTargets.Class)]
public class PluginLoadAttribute : Attribute
{
    public string Name { get; }
    public string[] DependsOn { get; }

    public PluginLoadAttribute(string name, params string[] dependsOn)
    {
        Name = name;
        DependsOn = dependsOn ?? Array.Empty<string>();
    }
}