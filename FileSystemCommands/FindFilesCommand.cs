using CommandLib;

namespace FileSystemCommands;

public class FindFilesCommand : ICommand
{
    private readonly string _directoryPath;
    private readonly string _mask;

    public FindFilesCommand(string directoryPath, string mask)
    {
        _directoryPath = directoryPath;
        _mask = mask;
    }

    public IReadOnlyList<string> FoundFiles { get; private set; } = new List<string>();

    public void Execute()
    {
        FoundFiles = Directory
            .GetFiles(_directoryPath, _mask, SearchOption.AllDirectories)
            .ToList();

        Console.WriteLine($"Найдено файлов по маске '{_mask}': {FoundFiles.Count}");
        FoundFiles.ToList().ForEach(Console.WriteLine);
    }
}