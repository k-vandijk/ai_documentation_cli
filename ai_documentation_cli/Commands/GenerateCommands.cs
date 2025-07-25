using ai_documentation_cli.Application.Interfaces;
using ai_documentation_cli.Application.Operations;
using Cocona;

namespace ai_documentation_cli.Commands;

// TODO: Add validation, for example, check if there are not too many files in the directory.

/// <summary>
/// Handles generation of documentation based on specified file or directory inputs.
/// </summary>
public class GenerateCommands
{
    private readonly IDocumentationGenerationService _documentationGenerationService;

    public GenerateCommands(IDocumentationGenerationService documentationGenerationService)
    {
        _documentationGenerationService = documentationGenerationService;
    }

    /// <summary>
    /// Asynchronously generates documentation for a specified file or directory.
    /// </summary>
    /// <param name="file">The path to the file for which documentation needs to be generated.</param>
    /// <param name="dir">The path to the directory containing files for which documentation needs to be generated.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    [Command("generate")]
    public async Task Execute([Option("file")] string? file, [Option("dir")] string? dir)
    {
        if (string.IsNullOrEmpty(file) && string.IsNullOrEmpty(dir))
        {
            // TODO use a more specific custom exception type.
            throw new InvalidOperationException("You must provide either a file or a directory to generate documentation.");
        }

        if (!string.IsNullOrEmpty(file) && !string.IsNullOrEmpty(dir))
        {
            // TODO use a more specific custom exception type.
            throw new InvalidOperationException("You cannot provide both a file and a directory to generate documentation.");
        }

        if (!string.IsNullOrEmpty(file))
        {
            await _documentationGenerationService.HandleDocumentationGenerationForFile(file);
        }
        else if (!string.IsNullOrEmpty(dir))
        {
            // TODO The following code is also used in ListCommands, refactor it to a common utility class.

            var extensions = new List<string> { ".cs" };
            var currentDirectory = Environment.CurrentDirectory;
            var directory = Path.Join(currentDirectory, dir);
            var relevantFiles = DirectoryOperations.ListRelevantFiles(directory, extensions);

            foreach (var relevantFile in relevantFiles)
            {
                await _documentationGenerationService.HandleDocumentationGenerationForFile(relevantFile, dir);
            }
        }
    }
}
