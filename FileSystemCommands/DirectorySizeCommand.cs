using CommandLib;

namespace FileSystemCommands;

public class DirectorySizeCommand : ICommand
{
    private readonly string _directoryPath;

    public DirectorySizeCommand(string directoryPath) => _directoryPath = directoryPath;

    public long TotalSize { get; private set; }

    public void Execute()
    {
        TotalSize = new DirectoryInfo(_directoryPath)
            .GetFiles("*", SearchOption.AllDirectories)
            .Sum(f => f.Length);

        Console.WriteLine($"Размер каталога '{_directoryPath}': {TotalSize} байт");
    }
}