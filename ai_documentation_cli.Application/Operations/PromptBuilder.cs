namespace ai_documentation_cli.Application.Operations;

public static class PromptBuilder
{
    public static string BuildPrompt(string code, string? existingSummary = null)
    {
        var prompt = existingSummary != null
            ? $"This is the function/class that I want to document: {code}\n\nAnd this is the existing summary: {existingSummary}"
            : $"This is the function/class that I want to document: {code}";

        return prompt;
    }
}