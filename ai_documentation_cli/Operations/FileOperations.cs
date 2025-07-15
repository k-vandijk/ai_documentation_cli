using System.Text.RegularExpressions;
using ai_documentation_cli.Dtos;

namespace ai_documentation_cli.Operations;

public static class FileOperations
{
    public static List<LineDto> GetFileLines(string path)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"The file '{path}' does not exist.");
        }

        return File.ReadLines(path).Select(l => new LineDto { UniqueIdentifier = GenerateShortUniqueIdentifier(), Content = l }).ToList();
    }

    public static ParsedFileDto ParseFileLines(List<LineDto> lines)
    {
        var parsedFile = new ParsedFileDto
        {
            Lines = lines,
        };

        // Regex to find the declaration line:
        var classRegex = new Regex(@"\b(class|record|struct)\s+\w+", RegexOptions.Compiled);
        var methodRegex = new Regex(@"^\s*(public|protected|internal|private)\s+(static\s+)?(async\s+)?[\w<>\[\],]+\s+\w+\s*\(.*?\)\s*$", RegexOptions.Compiled);

        // Walk through each line by index so we can skip ahead after extracting a block
        for (int i = 0; i < lines.Count; i++)
        {
            var content = lines[i].Content.Trim();

            // 1) Class declaration?
            if (classRegex.IsMatch(content))
            {
                // Grab the full block (from this line through its matching closing brace)
                var block = ExtractBlock(lines, ref i);

                parsedFile.Classes.Add(new ClassDocumentationDto
                {
                    Summary = string.Empty,   // to be filled later
                    Lines = block,
                });

                continue;
            }

            // 2) Method declaration?
            if (methodRegex.IsMatch(content))
            {
                var block = ExtractBlock(lines, ref i);

                // Only parse parameters & return type from the signature line:
                var sigLine = block[0].Content.Trim();
                var parameters = ParseParameters(sigLine);
                var returnType = ParseReturnType(sigLine);

                parsedFile.Functions.Add(new FunctionDocumentationDto
                {
                    Summary = string.Empty, // to be filled later
                    Lines = block,
                    Parameters = parameters,
                    ReturnType = new ReturnTypeDto { Description = returnType },
                });
            }
        }

        return parsedFile;
    }

    /// <summary>
    /// Starting at lines[startIndex], collects all lines from the opening brace
    /// through the matching closing brace (supports nested braces). Advances
    /// startIndex to the last line of the block so the caller can skip forward.
    /// </summary>
    private static List<LineDto> ExtractBlock(List<LineDto> lines, ref int startIndex)
    {
        var block = new List<LineDto>();
        int braceCount = 0;
        bool seenOpening = false;

        for (int j = startIndex; j < lines.Count; j++)
        {
            var line = lines[j];
            block.Add(line);

            // Count braces in this line
            foreach (var c in line.Content)
            {
                if (c == '{')
                {
                    braceCount++;
                    seenOpening = true;
                }

                if (c == '}')
                {
                    braceCount--;
                }
            }

            // Once we've seen at least one '{' and braces are balanced again, we're done
            if (seenOpening && braceCount == 0)
            {
                startIndex = j;  // advance outer loop past this block
                break;
            }
        }

        return block;
    }

    /// <summary>
    /// Parses "int x, string y" into a list of ParameterDto { Name = "x" / "y" }.
    /// </summary>
    private static List<ParameterDto> ParseParameters(string signatureLine)
    {
        var list = new List<ParameterDto>();
        var m = Regex.Match(signatureLine, @"\((.*?)\)");
        if (!m.Success || string.IsNullOrWhiteSpace(m.Groups[1].Value))
        {
            return list;
        }

        foreach (var raw in m.Groups[1].Value.Split(','))
        {
            var parts = raw.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length >= 2)
            {
                list.Add(new ParameterDto
                {
                    Name = parts.Last(),
                    Description = string.Empty,
                });
            }
        }

        return list;
    }

    private static string ParseReturnType(string signatureLine)
    {
        // e.g. "public static Task<string> Foo(...)" → ["public","static","Task<string>","Foo(...)"]
        var tokens = signatureLine
            .Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

        // return type is usually the 3rd token (after visibility + optional static/async)
        return tokens.Length >= 3
            ? tokens[2]
            : "void";
    }

    private static string GenerateShortUniqueIdentifier()
    {
        return Guid.NewGuid().ToString("N").Substring(0, 8);
    }
}