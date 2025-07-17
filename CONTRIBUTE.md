# Clean Architecture

This project follows the principles of Clean Architecture, which emphasizes separation of concerns and independence from frameworks.

- **Layers**:  
  1. **Domain** (core business rules)  
  2. **Application** (use‑case orchestrations)  
  3. **Infrastructure** (external implementations)  
  4. **Presentation** (UI, APIs, functions)

- **Dependency Rule**: outer layers depend only on inner ones.

- **Key Benefits**:  
  - Framework‑agnostic core  
  - Easy testing and swapping of UI/infrastructure  
  - Clear separation of concerns

# Environment variables

The following environment variables are required for the application to run:

```.env
AZURE_OPENAI_URL = x
AZURE_OPENAI_KEY = x
AZURE_DEPLOYMENT_NAME = x
```

# How to pack and install the package

```
dotnet pack --configuration Release
```

```
dotnet tool install --global --add-source ./nupkg ai_documentation_cli
```

```
dotnet tool uninstall -g ai_documentation_cli
```