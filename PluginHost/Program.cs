using PluginContracts;
using PluginHost;

string pluginDir = args.Length > 0 ? args[0] : AppContext.BaseDirectory;

Console.WriteLine($"Поиск плагинов в: {pluginDir}\n");

var discovered = PluginLoader.Discover(pluginDir);

if (discovered.Count == 0)
{
    Console.WriteLine("Плагины не найдены.");
    return;
}

Console.WriteLine("Обнаружены плагины: " +
    string.Join(", ", discovered.Select(p => p.Name)) + "\n");

var ordered = DependencyResolver.Sort(discovered);

Console.WriteLine("Порядок загрузки: " +
    string.Join(" -> ", ordered.Select(p => p.Name)) + "\n");

ordered
    .Select(p => (ICommand)Activator.CreateInstance(p.Type)!)
    .ToList()
    .ForEach(cmd => cmd.Execute());