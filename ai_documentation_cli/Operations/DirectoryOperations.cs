using Microsoft.Extensions.FileSystemGlobbing.Abstractions;

namespace ai_documentation_cli.Operations;

public static class DirectoryOperations
{
    public static List<string> ListRelevantFiles(string path, List<string> extensions)
    {
        var matcher = DocumentignoreOperations.BuildMatcher(path);
        var dirInfo = new DirectoryInfo(path);
        var result = matcher.Execute(new DirectoryInfoWrapper(dirInfo));

        var matched = result.Files
            .Select(f => f.Path)
            .Where(p => extensions
                .Contains(Path.GetExtension(p)
                    .ToLowerInvariant()));

        return matched.OrderBy(m => m).ToList();
    }
}