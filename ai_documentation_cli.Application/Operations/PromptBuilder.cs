namespace ai_documentation_cli.Application.Operations;

/// <summary>
/// Provides methods to build prompts for documenting classes or functions.
/// </summary>
public static class PromptBuilder
{
    /// <summary>
    /// Builds a prompt message for documenting a function or class with the given code and optional existing summary.
    /// </summary>
    /// <param name="code">The code representing the function or class to be documented.</param>
    /// <param name="existingSummary">The existing summary of the function or class, if available.</param>
    /// <returns>A prompt message including the code and existing summary for documenting a function or class.</returns>
    public static string BuildPrompt(string code, string? existingSummary = null)
    {
        var prompt = string.IsNullOrEmpty(existingSummary)
            ? $"xThis is the function/class that I want to document: {code}\n\nThere is no existing summary. Please provide one."
            : $"This is the function/class that I want to document: {code}\n\nAnd this is the existing summary: {existingSummary}";

        return prompt;
    }
}
