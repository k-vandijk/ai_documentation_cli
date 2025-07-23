using ai_documentation_cli.Application.Interfaces;
using ai_documentation_cli.Application.Operations;
using Cocona;

namespace ai_documentation_cli.Commands;

// TODO: Add validation, for example, check if there are not too many files in the directory.
// TODO: Make sure the Xml is added before the function attributes.

public class GenerateCommands
{
    private readonly IDocumentationGenerationService _documentationGenerationService;

    public GenerateCommands(IDocumentationGenerationService documentationGenerationService)
    {
        _documentationGenerationService = documentationGenerationService;
    }

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
