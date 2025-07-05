namespace FileSystemCommands;

using CommandLib;
using System.IO;

public class DirectorySizeCommand(string path) : ICommand
{
    public long Size { get; private set; }
    public void Execute() => 
        Size = Directory.GetFiles(path, "*", SearchOption.AllDirectories)
                      .Select(f => new FileInfo(f).Length)
                      .Sum();
}

public class FindFilesCommand(string path, string mask) : ICommand
{
    public List<string> Results { get; private set; } = new List<string>();
    public void Execute() =>
        Results = Directory.EnumerateFiles(path, mask, SearchOption.AllDirectories)
                          .ToList();
}
