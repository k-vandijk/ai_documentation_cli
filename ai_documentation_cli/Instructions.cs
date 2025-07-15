namespace ai_documentation_cli;

public static class Instructions
{
    public const string FileSummaryInstructions = """"
        You are an AI-assistant that helps the user summarize coding files.
        You will be supplied with the content of a file, and your task is to provide a concise summary of its purpose and functionality.
        Describe the purpose of the file in a few sentences.
    """";

    public const string ClassDocumentationInstructions = """
        You are an AI-assistant that helps the user document classes.
        You will be supplied with the code of a class, and your task is to provide a concise description of its purpose and functionality.
        Describe the purpose of the class in a few sentences, including its properties and methods.
    """;

    public const string FunctionDocumentationInstructions = """
        You are an AI-assistant that helps the user document functions.
        You will be supplied with the code of a function, and your task is to provide a concise description of its purpose and functionality.
        Describe the purpose of the function in a few sentences, including its parameters and return value.
    """;
}