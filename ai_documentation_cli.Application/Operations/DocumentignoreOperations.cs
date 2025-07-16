using Microsoft.Extensions.FileSystemGlobbing;

namespace ai_documentation_cli.Application.Operations;

public static class DocumentignoreOperations
{
    public static Matcher BuildMatcher(string rootPath)
    {
        var patterns = ReadDocumentIgnorePatterns(rootPath);
        var matcher = new Matcher(StringComparison.OrdinalIgnoreCase);
        matcher.AddInclude("**/*");

        foreach (var pattern in patterns)
        {
            if (pattern.StartsWith("!"))
            {
                matcher.AddInclude(pattern.Substring(1));
            }
            else
            {
                matcher.AddExclude(pattern);
            }
        }

        return matcher;
    }

    private static List<string> ReadDocumentIgnorePatterns(string path)
    {
        var ignoreFile = Path.Combine(path, ".documentignore");

        if (!File.Exists(ignoreFile))
        {
            return new List<string>();
        }

        return File.ReadAllLines(ignoreFile)
            .Select(line => line.Trim())
            .Where(line => line.Length > 0 && !line.StartsWith("#"))
            .ToList();
    }
}