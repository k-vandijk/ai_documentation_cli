using ai_documentation_cli.Operations;
using Cocona;

namespace ai_documentation_cli.Commands;

public class ListCommands
{
    [Command("list")]
    public void Execute([Option("dir")] string? dir)
    {
        var extensions = new List<string> { ".cs", ".md" };

        var currentDirectory = Environment.CurrentDirectory;
        var directory = Path.Join(currentDirectory, dir);

        var relevantFiles = DirectoryOperations.ListRelevantFiles(directory, extensions);

        Console.WriteLine($"\nFound {relevantFiles.Count} relevant files: \n");

        foreach (var file in relevantFiles)
        {
            Console.WriteLine(file);
        }
    }
}