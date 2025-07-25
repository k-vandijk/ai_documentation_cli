# Contents

- [Introduction & purpose](#introduction--purpose)
- [Environment variables](#environment-variables)
- [List of commands](#list-of-commands)
- [.documentignore example](#documentignore-example)

# Introduction & purpose

A command line interface for generating documentation using AI.

The application will search every .cs file in the current directory and generate documentation for each class, method, and property using OpenAi's ChatGPT.

# Environment variables

Set the following environment variables in the `.env` file:

```.env
AZURE_OPENAI_URL=<YOUR_AZOPENAI_URL>
AZURE_OPENAI_KEY=<YOUR_AZOPENAI_KEY>
AZURE_DEPLOYMENT_NAME=gpt-35-turbo
```

# List of commands

### `generate --file <filename>`
Generate documentation for a specific file

### `generate --dir <directory>`
Generate documentation for all files in the specified directory

### `list --dir <directory>`
List all relevant files in the current directory, or if a directory is specified, in that directory.

# .documentignore example

You can manually ignore files or directories from being processed by the documentation generator by creating a `.documentignore` file in the root directory of your project.

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
