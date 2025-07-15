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

        var fileLines = FileOperations.GetFileLines(file);

        var parsedFile = FileOperations.ParseFileLines(fileLines);

        foreach (var classDoc in parsedFile.Classes)
        {
            var joinedLines = string.Join("\n", classDoc.Lines.Select(l => l.Content));

            var classSummary = await _chatCompletionService.GetChatCompletionAsync(joinedLines, Instructions.ClassDocumentationInstructions);

            classDoc.Summary = classSummary;

            Console.WriteLine($"\n{joinedLines}\n");

            Console.WriteLine($"\n{classDoc.Summary}\n");
        }

        foreach (var functionDoc in parsedFile.Functions)
        {
            var joinedLines = string.Join("\n", functionDoc.Lines.Select(l => l.Content));
            var functionSummary = await _chatCompletionService.GetChatCompletionAsync(joinedLines, Instructions.FunctionDocumentationInstructions);

            functionDoc.Summary = functionSummary;

            Console.WriteLine($"\n{joinedLines}\n");

            Console.WriteLine($"\n{functionDoc.Summary}\n");
        }
    }
}