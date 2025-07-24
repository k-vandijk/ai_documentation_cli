using ai_documentation_cli.Application.Operations;
using Cocona;

namespace ai_documentation_cli.Commands;

/// <summary>
/// A class that contains a method to list relevant files in a specified directory using a given extension.
/// </summary>
public class ListCommands
{
    /// <summary>
    /// Executes a function to list files with specific extensions in a specified directory.
    /// </summary>
    /// <param name="dir">The directory in which to search for files.</param>
    [Command("list")]
    public void Execute([Option("dir")] string? dir)
    {
        var extensions = new List<string> { ".cs" };

        var currentDirectory = Environment.CurrentDirectory;
        var directory = Path.Join(currentDirectory, dir);

        var relevantFiles = DirectoryOperations.ListRelevantFiles(directory, extensions);

        foreach (var file in relevantFiles)
        {
            Console.WriteLine(file);
        }
    }
}
