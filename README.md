# Introduction & purpose

A command line interface for generating documentation using AI.

The application will search every .cs file in the current directory and generate documentation for each class, method, and property using OpenAi's ChatGPT.

# List of commands

## `generate --file <filename>`
Generate documentation for a specific .cs file.

## `list`
List all .cs files in the current directory.

# `.documentignore` example

``` plaintext
# Ignore build output
bin/
obj/

# Ignore designer files
*.Designer.cs

# Ignore all test projects
**/Tests/**

# But keep this one single file
!MyApp/Tests/Integration/KeepMe.txt

```