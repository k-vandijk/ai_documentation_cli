using ai_documentation_cli.Application.Operations;
using Cocona;

namespace ai_documentation_cli.App.Commands;

/// <summary>
/// Represents a class that provides functionality to list relevant files within a specified directory based on given extensions.
/// </summary>
public class ListCommands
{
    [Command("list")]
    /// <summary>
    /// Executes the operation of listing relevant files with specified extensions in a given directory.
    /// </summary>
    /// <param name="dir">The directory path where the relevant files will be searched.</param>
    /// <returns>No return value.</returns>
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
