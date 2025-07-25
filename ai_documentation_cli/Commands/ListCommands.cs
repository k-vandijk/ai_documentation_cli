using ai_documentation_cli.Application.Operations;
using Cocona;

namespace ai_documentation_cli.Commands;

/// <summary>
/// Represents a class that executes a command to list relevant files in a specified directory.
/// </summary>
public class ListCommands
{
    /// <summary>
    /// Executes the function to list relevant files with specific extensions in the specified directory.
    /// </summary>
    /// <param name="dir">The directory path where the relevant files are located.</param>
    [Command("list")]
    public void Execute([Option("dir")] string? dir)
    {
        // TODO The following code is also used in GenerateCommands, refactor it to a common utility class.

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
