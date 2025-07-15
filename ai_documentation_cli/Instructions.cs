namespace ai_documentation_cli;

public static class Instructions
{
    public const string ClassDocumentationInstructions = """"
        You are an AI-assistant that helps the user document classes.
        You will be supplied with the content of a file, and your task is to provide a concise description of its purpose and functionality.
        Describe the purpose of the class in a few sentences.
        
        The summary should be in the following format: /// <summary>\n/// <purpose of the class>\n/// </summary>
    """";

    public const string FunctionDocumentationInstructions = """
        You are an AI-assistant that helps the user document functions.
        You will be supplied with the code of a function, and your task is to provide a concise description of its purpose and functionality.
        Describe the purpose of the function in a few sentences, including its parameters and return value.
        
        The summary should be in the following format, repeat the param section as often as needed: 
        /// <summary>\n/// [purpose of the function]\n/// </summary>\n/// <param name="[param]">[description of the param]</param>\n/// <returns>[the return value of the function]</returns>
    """;
}