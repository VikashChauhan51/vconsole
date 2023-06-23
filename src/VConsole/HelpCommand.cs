namespace VConsole;

[Verb("help", HelpText = "Help about command(s).")]
public class HelpCommand : ICommand
{
    [Option('c', "command", Required = true, HelpText = "Name of the command.")]
    public string Command { get; set; } = string.Empty;
    public void Execute()
    {
        Console.WriteLine("... \n");
    }
}
