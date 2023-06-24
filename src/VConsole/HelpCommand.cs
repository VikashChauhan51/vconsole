namespace VConsole;

[Verb("help", new string[] { "--help" }, HelpText = "Help about command(s).")]
internal sealed class HelpCommand : ICommand
{
    [Option('c', "command", Required = false, HelpText = "Name of the command.")]
    public string Command { get; set; } = string.Empty;
    public void Execute()
    {
        Console.WriteLine("... \n");
    }
}
