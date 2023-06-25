
using VConsole.Core;

namespace VConsole.App.Commands;

[Verb("push", HelpText = "Save all your commits to the cloud.")]
internal class Push : ICommand
{
    public Task Execute()
    {
        Console.WriteLine("Push command executing.");

        return Task.CompletedTask;
    }
}
