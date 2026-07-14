namespace PluginHost;

public class PluginInfo
{
    public required string Name { get; init; }
    public required string[] DependsOn { get; init; }
    public required Type Type { get; init; }
}