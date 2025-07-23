using System.Text.RegularExpressions;
using ai_documentation_cli.Domain.Interfaces;
using ai_documentation_cli.Domain.Models;

namespace ai_documentation_cli.Application.Operations;

/// <summary>
/// A utility class containing methods for parsing contents of C# code files to extract class and method documentation.
/// </summary>
public static class FileParser
{
    /// <summary>
    /// Retrieves the lines from a specified file and returns a list of Line objects.
    /// </summary>
    /// <param name="path">The path to the file from which to read the lines.</param>
    /// <returns>A list of Line objects containing unique identifiers and the content of each line in the file.</returns>
    public static List<Line> GetFileLines(string path)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"The file '{path}' does not exist.");
        }

        return File.ReadLines(path).Select(l => new Line { UniqueIdentifier = UniqueIdentifierGenerator.GenerateShortUniqueIdentifier(), Content = l }).ToList();
    }

    /// <summary>
    /// Parses the list of lines to extract class, record, or struct definitions in C# code.
    /// </summary>
    /// <param name="lines">The list of Line objects representing lines of code to parse.</param>
    /// <returns>A list of ClassDocumentation objects containing information about the parsed classes.</returns>
    public static List<ClassDocumentation> ParseClasses(List<Line> lines)
    {
        // This regex matches class, record, or struct definitions in C# code.
        // It looks for the keywords 'class', 'record', or 'struct' followed by one or more whitespace characters and then a valid identifier (name).
        // It does not match generic types or nested classes.
        // It assumes that the class definition is on a single line, which is common in C#.
        var classRegex = new Regex(@"\b(class|record|struct)\s+\w+", RegexOptions.Compiled);

        var classes = ParseGeneric<ClassDocumentation>(lines, classRegex);

        return classes;
    }

    /// <summary>
    /// Parses a list of lines containing C# code to extract and document method definitions.
    /// </summary>
    /// <param name="lines">The list of lines containing C# code to search for method definitions.</param>
    /// <returns>A list of FunctionDocumentation objects representing the parsed method definitions.</returns>
    public static List<FunctionDocumentation> ParseFunctions(List<Line> lines)
    {
        // This regex matches method definitions in C# code.
        // It looks for access modifiers (public, protected, internal, private), optional static and async keywords,
        // followed by a return type (which can include generics), a method name, and a parameter list.
        // It does not match method signatures that are split across multiple lines.
        var methodRegex = new Regex(@"^\s*(public|protected|internal|private)\s+(static\s+)?(async\s+)?[\w<>\[\],]+\s+\w+\s*\(.*?\)\s*$", RegexOptions.Compiled);

        var functions = ParseGeneric<FunctionDocumentation>(lines, methodRegex);

        return functions;
    }

    /// <summary>
    /// Retrieves the XML documentation lines located above a specific line identified by its unique identifier in a list of lines.
    /// </summary>
    /// <param name="lineId">The unique identifier of the line to search for.</param>
    /// <param name="lines">The list of lines where the specific line is located.</param>
    /// <returns>A list of Line objects that represent the XML documentation lines above the line with the specified unique identifier.</returns>
    public static List<Line> GetXmlDocumentationLinesAt(string lineId, List<Line> lines)
    {
        int i = lines.FindIndex(l => l.UniqueIdentifier == lineId) - 1; // Start from the line before the method or class declaration
        var result = new List<Line>();
        while (i >= 0 && lines[i].Content.TrimStart().StartsWith("///"))
        {
            result.Insert(0, lines[i]);
            i--;
        }

        return result;
    }

    private static List<T> ParseGeneric<T>(List<Line> lines, Regex regex) 
        where T : IDocumentable, new()
    {
        var result = new List<T>();

        for (int i = 0; i < lines.Count; i++)
        {
            var trimmed = lines[i].Content.TrimStart();
            if (trimmed.StartsWith("//"))
            {
                continue;
            }

            if (regex.IsMatch(lines[i].Content.Trim()))
            {
                var block = ExtractBlock(lines, ref i);

                result.Add(new T
                {
                    Summary = string.Empty,
                    Lines = block,
                });
            }
        }

        foreach (var x in result)
        {
            var summaryLines = GetXmlDocumentationLinesAt(x.Lines.First().UniqueIdentifier, lines);

            if (summaryLines.Any())
            {
                x.Summary = string.Join("\n", summaryLines.Select(l => l.Content.Trim()));
            }
        }

        return result;
    }

    /// <summary>
    /// Extracts a block of lines starting from the current index until the corresponding closing brace is found.
    /// </summary>
    /// <param name="lines">The list of lines from which to extract the block.</param>
    /// <param name="index">The index to start extracting the block from. This will be updated to the index after the extracted block.</param>
    /// <returns>A list of lines representing the extracted block.</returns>
    private static List<Line> ExtractBlock(List<Line> lines, ref int index)
    {
        var block = new List<Line>();
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
