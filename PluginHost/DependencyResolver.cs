namespace PluginHost;

public static class DependencyResolver
{
    public static IReadOnlyList<PluginInfo> Sort(IReadOnlyList<PluginInfo> plugins)
    {
        var byName = plugins.ToDictionary(p => p.Name);
        var sorted = new List<PluginInfo>();
        var visited = new Dictionary<string, bool>(); 

        void Visit(PluginInfo plugin)
        {
            if (visited.TryGetValue(plugin.Name, out bool done))
            {
                if (!done)
                    throw new InvalidOperationException(
                        $"Обнаружена циклическая зависимость с участием '{plugin.Name}'");
                return;
            }

            visited[plugin.Name] = false; 

            plugin.DependsOn
                .Where(byName.ContainsKey)
                .Select(dep => byName[dep])
                .ToList()
                .ForEach(Visit);

            visited[plugin.Name] = true;  
            sorted.Add(plugin);
        }

        plugins.ToList().ForEach(Visit);
        return sorted;
    }
}