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
}