

using VConsole.Core;

namespace VConsole.App.Commands;

[Verb("commit", HelpText = "Save code changes")]
internal class Commit : ICommand
{
    [Option('m', "message", Required = true, HelpText = "Explain what code changes you did.")]
    public string Message { get; set; } = string.Empty;
    public Task Execute()
    {
        Console.WriteLine($"Executing command with message: {Message}");
        return Task.CompletedTask;
    }
}
