using PluginContracts;

namespace SamplePlugins;

[PluginLoad("Logger")]
public class LoggerPlugin : ICommand
{
    public void Execute() => Console.WriteLine("[Logger] инициализация логирования");
}

[PluginLoad("Database", "Logger")]
public class DatabasePlugin : ICommand
{
    public void Execute() => Console.WriteLine("[Database] подключение к БД (после Logger)");
}

[PluginLoad("Cache", "Database")]
public class CachePlugin : ICommand
{
    public void Execute() => Console.WriteLine("[Cache] прогрев кэша (после Database)");
}

public class NotAPlugin : ICommand
{
    public void Execute() => Console.WriteLine("[NotAPlugin] меня не должно быть в выводе");
}