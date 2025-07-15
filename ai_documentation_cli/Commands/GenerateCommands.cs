using ai_documentation_cli.Interfaces;
using ai_documentation_cli.Operations;
using Cocona;

namespace ai_documentation_cli.Commands;

public class GenerateCommands
{
    private readonly IChatCompletionService _chatCompletionService;

    public GenerateCommands(IChatCompletionService chatCompletionService)
    {
        _chatCompletionService = chatCompletionService;
    }

    [Command("generate")]
    public async Task Execute([Option("file")] string? file, [Option("query")] string? additionalQuery)
    {
        if (string.IsNullOrEmpty(file))
        {
            throw new ArgumentException("The 'file' option must be provided.");
        }

        var fileContent = FileOperations.GetFileContent(file);

        Console.WriteLine("\nGenerating file summary...\n");

        var fileSummary = await _chatCompletionService.GetChatCompletionAsync(fileContent, $"{Instructions.FileSummaryInstructions} {additionalQuery}");

        Console.WriteLine(fileSummary);

        var fileSummaryLines = fileSummary.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

        FileOperations.InsertBeforeLineWithPrefix(fileSummaryLines.ToList(), "public static class", file);
    }
}