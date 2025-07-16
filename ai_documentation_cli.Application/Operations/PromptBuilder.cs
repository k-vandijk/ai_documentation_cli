namespace ai_documentation_cli.Application.Operations;

public static class PromptBuilder
{
    public static string BuildPrompt(string code, string? existingSummary = null)
    {
        var prompt = string.IsNullOrEmpty(existingSummary)
            ? $"xThis is the function/class that I want to document: {code}\n\nThere is no existing summary. Please provide one."
            : $"This is the function/class that I want to document: {code}\n\nAnd this is the existing summary: {existingSummary}";

        return prompt;
    }
}