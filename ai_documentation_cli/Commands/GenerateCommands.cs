using ai_documentation_cli.Application.Interfaces;
using ai_documentation_cli.Application.Operations;
using Cocona;

namespace ai_documentation_cli.Commands;

// TODO: Add validation, for example, check if there are not too many files in the directory.

/// <summary>
/// Generates documentation for C# files based on specified file or directory input.
/// </summary>
public class GenerateCommands
{
    private readonly IDocumentationGenerationService _documentationGenerationService;

    public GenerateCommands(IDocumentationGenerationService documentationGenerationService)
    {
        _documentationGenerationService = documentationGenerationService;
    }

    /// <summary>
    /// Generates documentation for a specified file or directory.
    /// </summary>
    /// <param name="file">The file for which documentation will be generated. Either this or the 'dir' parameter must be provided.</param>
    /// <param name="dir">The directory containing files for which documentation will be generated. Either this or the 'file' parameter must be provided.</param>
    /// <returns>A Task representing the asynchronous operation of generating documentation.</returns>
    [Command("generate")]
    public async Task Execute([Option("file")] string? file, [Option("dir")] string? dir)
    {
        if (string.IsNullOrEmpty(file) && string.IsNullOrEmpty(dir))
        {
            throw new InvalidOperationException("You must provide either a file or a directory to generate documentation.");
        }

        if (!string.IsNullOrEmpty(file) && !string.IsNullOrEmpty(dir))
        {
            throw new InvalidOperationException("You cannot provide both a file and a directory to generate documentation.");
        }

        if (!string.IsNullOrEmpty(file))
        {
            await _documentationGenerationService.HandleDocumentationGenerationForFile(file);
        }
        else if (!string.IsNullOrEmpty(dir))
        {
            var extensions = new List<string> { ".cs" };
            var currentDirectory = Environment.CurrentDirectory;
            var directory = Path.Join(currentDirectory, dir);
            var relevantFiles = DirectoryOperations.ListRelevantFiles(directory, extensions);

            foreach (var relevantFile in relevantFiles)
            {
                await _documentationGenerationService.HandleDocumentationGenerationForFile(relevantFile);
            }
        }
    }
}
