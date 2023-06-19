using VConsole.Core;

namespace VConsole.App.Commands;

[Verb("clone", HelpText = "Clone a repository into a new directory.")]
internal class Clone : ICommand
{
    [Option('u', "url", Required = true, HelpText = "Cloud repository URL.")]
    public string URL { get; set; } = string.Empty;
    public void Execute()
    {
        Console.WriteLine($"Cloning a repository: {URL}");
    }
}
