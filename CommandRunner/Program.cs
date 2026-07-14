using System.Reflection;
using CommandLib;

string dllPath = Path.Combine(AppContext.BaseDirectory, "FileSystemCommands.dll");

if (!File.Exists(dllPath))
{
    Console.WriteLine($"Не найдена библиотека команд: {dllPath}");
    return;
}

Assembly assembly = Assembly.LoadFrom(dllPath);

string demoDir = Path.GetTempPath();

Type sizeType = assembly.GetType("FileSystemCommands.DirectorySizeCommand")!;
ICommand sizeCommand = (ICommand)Activator.CreateInstance(sizeType, demoDir)!;
sizeCommand.Execute();

Type findType = assembly.GetType("FileSystemCommands.FindFilesCommand")!;
ICommand findCommand = (ICommand)Activator.CreateInstance(findType, demoDir, "*.txt")!;
findCommand.Execute();