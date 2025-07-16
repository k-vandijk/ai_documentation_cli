using ai_documentation_cli.Commands;
using Cocona;
using kvandijk.Common.Utils;

DotenvLoader.Load(".env");

var builder = CoconaApp.CreateBuilder();

var app = builder.Build();

app.AddCommands<GenerateCommands>();
app.AddCommands<ListCommands>();

await app.RunAsync();