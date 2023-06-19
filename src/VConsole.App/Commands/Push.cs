
using VConsole.Core;

namespace VConsole.App.Commands;

[Verb("push", HelpText = "Save all your commits to the cloud.")]
internal class Push : ICommand
{
    public void Execute()
    {
        Console.WriteLine("Push command executing.");
    }
}