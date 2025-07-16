using ai_documentation_cli.Dtos;
using ai_documentation_cli.Interfaces;
using ai_documentation_cli.Operations;
using Cocona;

namespace ai_documentation_cli.Commands;

public class GenerateCommands
{
    //private readonly IChatCompletionService _chatCompletionService;

    //public GenerateCommands(IChatCompletionService chatCompletionService)
    //{
    //    _chatCompletionService = chatCompletionService;
    //}

    [Command("generate")]
    public void Execute([Option("file")] string? file, [Option("query")] string? additionalQuery)
    {
        if (string.IsNullOrEmpty(file))
        {
            throw new ArgumentException("The 'file' option must be provided.");
        }

        var lines = FileOperations.GetFileLines(file);

        var classes = FileOperations.ParseClasses(lines);
        var functions = FileOperations.ParseFunctions(lines);

        //var parsedFile = new ParsedFileDto { Lines = lines, Classes = classes, Functions = functions, };

        foreach (var c in classes)
        {
            Console.WriteLine(c.Lines.First().Content.Trim());
        }

        foreach (var f in functions)
        {
            var functionString = string.Join("\n", f.Lines.Select(l => l.Content));

            Console.WriteLine(functionString);
        }
    }
}