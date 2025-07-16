using ai_documentation_cli.Dtos;

namespace ai_documentation_cli.Operations;

public static class FileInserter
{
    public static List<LineDto> InsertLinesAt(string lineId, List<LineDto> linesToInsert, List<LineDto> lines)
    {
        if (lines == null || lines.Count == 0)
        {
            throw new ArgumentException("The lines to insert cannot be null or empty.");
        }

        var result = new List<LineDto>();
        foreach (var line in lines)
        {
            if (line.UniqueIdentifier == lineId)
            {
                result.AddRange(linesToInsert);
            }

            result.Add(line);
        }

        return result;
    }

    public static List<LineDto> SplitSummaryIntoLines(string summary)
    {
        var lines = summary.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

        return lines.Select(line => new LineDto
        {
            UniqueIdentifier = UniqueIdentifierGenerator.GenerateShortUniqueIdentifier(),
            Content = line.Trim(),
        }).ToList();
    }
}