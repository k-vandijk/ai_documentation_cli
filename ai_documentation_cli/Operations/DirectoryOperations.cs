using Microsoft.Extensions.FileSystemGlobbing.Abstractions;

namespace ai_documentation_cli.Operations;

/// <summary>
/// This file contains a static class `DirectoryOperations` that provides a method `ListRelevantFiles` to list files with specific extensions in a given directory.
/// </summary>
/// <summary>
/// This file defines a static class `DirectoryOperations` that contains a method `ListRelevantFiles` used to list files with specific extensions in a given directory.
/// </summary>
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
