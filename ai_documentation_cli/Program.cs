using ai_documentation_cli.App.Commands;
using ai_documentation_cli.Infrastructure;
using Cocona;
using kvandijk.Common.Utils;

#if DEBUG
DotenvLoader.Load(".env");
#endif

var builder = CoconaApp.CreateBuilder();

builder.Services.AddInfrastructure();

var app = builder.Build();

app.AddCommands<GenerateCommands>();
app.AddCommands<ListCommands>();

await app.RunAsync();