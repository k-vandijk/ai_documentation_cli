using ai_documentation_cli.Domain.Dtos;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Text.RegularExpressions;

namespace ai_documentation_cli.Application.Operations;

/// <summary>
/// This class provides methods for parsing a file containing C# code to extract class and method definitions along with their documentation.
/// It includes functionality to read lines from a file, parse classes, parse functions, and extract code blocks for classes or functions.
/// </summary>
public static class FileParser
{
    /// <summary>
    /// Retrieves the lines of a file specified by the given path and creates a list of LineDto objects.
    /// </summary>
    /// <param name="path">The file path from which to read the lines.</param>
    /// <returns>A list of LineDto objects containing unique identifiers and content of each line in the file.</returns>
    public static List<LineDto> GetFileLines(string path)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"The file '{path}' does not exist.");
        }

        return File.ReadLines(path).Select(l => new LineDto { UniqueIdentifier = UniqueIdentifierGenerator.GenerateShortUniqueIdentifier(), Content = l }).ToList();
    }

    /// <summary>
    /// This function parses a list of LineDto objects to extract class, record, or struct definitions in C# code.
    /// It uses a regex pattern to match lines containing the keywords 'class', 'record', or 'struct' followed by a valid identifier.
    /// The function ignores lines starting with '//', does not match generic types or nested classes, and assumes class definitions are on a single line.
    /// </summary>
    /// <param name="lines">A list of LineDto objects representing lines of code to parse.</param>
    /// <returns>A list of ClassDocumentationDto objects representing the extracted class, record, or struct definitions.</returns>
    public static List<ClassDocumentationDto> ParseClasses(List<LineDto> lines)
    {
        // This regex matches class, record, or struct definitions in C# code.
        // It looks for the keywords 'class', 'record', or 'struct' followed by one or more whitespace characters and then a valid identifier (name).
        // It does not match generic types or nested classes.
        // It assumes that the class definition is on a single line, which is common in C#.
        var classRegex = new Regex(@"\b(class|record|struct)\s+\w+", RegexOptions.Compiled);
        var classes = new List<ClassDocumentationDto>();

        for (int i = 0; i < lines.Count; i++)
        {
            var trimmed = lines[i].Content.TrimStart();
            if (trimmed.StartsWith("//"))
            {
                continue;
            }

            if (classRegex.IsMatch(lines[i].Content.Trim()))
            {
                var block = ExtractBlock(lines, ref i);

                classes.Add(new ClassDocumentationDto
                {
                    Summary = string.Empty,
                    Lines = block,
                });
            }
        }

        return classes;
    }

    /// <summary>
    /// This function parses and extracts function definitions from a list of lines containing C# code.
    /// </summary>
    /// <param name="lines">A list of LineDto objects representing lines of C# code to be processed.</param>
    /// <returns>A list of FunctionDocumentationDto objects containing information about the parsed functions.</returns>
    public static List<FunctionDocumentationDto> ParseFunctions(List<LineDto> lines)
    {
        // This regex matches method definitions in C# code.
        // It looks for access modifiers (public, protected, internal, private), optional static and async keywords,
        // followed by a return type (which can include generics), a method name, and a parameter list.
        // It does not match method signatures that are split across multiple lines.
        var methodRegex = new Regex(@"^\s*(public|protected|internal|private)\s+(static\s+)?(async\s+)?[\w<>\[\],]+\s+\w+\s*\(.*?\)\s*$", RegexOptions.Compiled);
        var functions = new List<FunctionDocumentationDto>();

        for (int i = 0; i < lines.Count; i++)
        {
            var trimmed = lines[i].Content.TrimStart();
            if (trimmed.StartsWith("//"))
            {
                continue;
            }

            if (methodRegex.IsMatch(lines[i].Content.Trim()))
            {
                var block = ExtractBlock(lines, ref i);

                var sigLine = block.FirstOrDefault()?.Content.Trim() ?? string.Empty;

                var parameters = ParseParametersWithRoslyn(sigLine);
                var returnType = ParseReturnTypeWithRoslyn(sigLine);

                functions.Add(new FunctionDocumentationDto
                {
                    Summary = string.Empty,
                    Lines = block,
                    Parameters = parameters,
                    ReturnType = returnType,
                });
            }
        }

        return functions;
    }

    private static ReturnTypeDto ParseReturnTypeWithRoslyn(string methodSignature)
    {
        var dummyCode = $"class Dummy {{ {methodSignature} {{ }} }}";
        var tree = CSharpSyntaxTree.ParseText(dummyCode);
        var root = tree.GetRoot();

        var method = root.DescendantNodes()
            .OfType<MethodDeclarationSyntax>()
            .FirstOrDefault();

        if (method == null)
        {
            return new ReturnTypeDto { Description = "unknown" };
        }

        return new ReturnTypeDto
        {
            Description = method.ReturnType.ToString(),
        };
    }

    private static List<ParameterDto> ParseParametersWithRoslyn(string methodSignature)
    {
        var fullMethod = $"class Dummy {{ {methodSignature} {{ }} }}";
        var tree = CSharpSyntaxTree.ParseText(fullMethod);
        var root = tree.GetRoot();

        var method = root.DescendantNodes().OfType<MethodDeclarationSyntax>().FirstOrDefault();
        if (method == null)
        {
            return new List<ParameterDto>();
        }

        return method.ParameterList.Parameters
            .Select(p => new ParameterDto
            {
                Name = p.Identifier.Text,
                Type = p.Type?.ToString() ?? string.Empty,
            })
            .ToList();
    }

    /// <summary>
    /// This function extracts a block of code from the list of lines, starting from the current index.
    /// It counts the opening and closing braces to determine when the block ends.
    /// </summary>
    /// <param name="lines">The lines for the function to loop through; Generally all lines of the file.</param>
    /// <param name="index">The starting line, generally the declaration of a class/function.</param>
    /// <returns>The full list of lines of the class/function.</returns>
    private static List<LineDto> ExtractBlock(List<LineDto> lines, ref int index)
    {
        var block = new List<LineDto>();
        int braceCount = 0;
        bool started = false;

        for (; index < lines.Count; index++)
        {
            var line = lines[index];
            block.Add(line);

            foreach (char c in line.Content)
            {
                if (c == '{')
                {
                    braceCount++;
                    started = true;
                }
                else if (c == '}')
                {
                    braceCount--;
                }
            }

            if (started && braceCount == 0)
            {
                break;
            }
        }

        return block;
    }
}
