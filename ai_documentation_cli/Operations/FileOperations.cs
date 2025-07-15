namespace ai_documentation_cli.Operations;

public static class FileOperations
{
    public static string GetFileContent(string path)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"The file '{path}' does not exist.");
        }

        return File.ReadAllText(path);
    }

    public static List<string> GetFileLines(string path)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"The file '{path}' does not exist.");
        }

        return File.ReadLines(path).ToList();
    }

    public static void InsertBeforeLineWithPrefix(List<string> insertLines, string destinationPrefix, string path)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"The file '{path}' does not exist.");
        }

        var updatedLines = File.ReadLines(path)
            .SelectMany(line => line.StartsWith(destinationPrefix)
                ? insertLines.Append(line)
                : new[] { line })
            .ToList();

        File.WriteAllLines(path, updatedLines);
    }
}