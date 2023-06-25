using System.Reflection;
using VConsole.Core;

namespace VConsole;

[Verb("version",new string[] { "--version" }, HelpText = "CLI app version information.")]
internal sealed class VersionCommand : ICommand
{
    public Task Execute()
    {
        AssemblyHelper.PrintInfo(new ConsoleHost());

        return Task.CompletedTask;
    }
}
