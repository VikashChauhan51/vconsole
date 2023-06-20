# VConsole
VConsole is a .NET library to parse command line arguments and execute commands.

The **VConsole** command Line Parser Library offers CLR applications a clean and concise API for manipulating command line arguments and related tasks, such as defining switches, options and verb commands. It allows you to display a help screen with a high degree of customization and a simple way to report syntax errors to the end user.

```
 NuGet Install VConsole
```

## Quick Start Examples
- Create a class to define valid `command` with `varb` and `options` **attrbutes** to receive the parsed options.
- Register commands using `RegisterCommand` or `RegisterCommandsFromAssembly` methods.
- Call `ParseArguments` with the `args` string array.

Example:

``` C#

[Verb("clone", HelpText = "Clone a repository into a new directory.")]
public class Clone : ICommand
{
    [Option('u', "url", Required = true, HelpText = "Cloud repository URL.")]
    public string URL { get; set; } = string.Empty;
    public void Execute()
    {
        Console.WriteLine($"Cloning a repository: {URL}");
    }
}

# top level statment in dotnet core
Parser.Default
    .RegisterCommand<Clone>()
    .ParseArguments(args);

or

static void Main(string[] args)
{
    Parser.Default
    .RegisterCommand<Clone>()
    .ParseArguments(args);
}

```
- Build your application and run it like this:
    ```cmd

    myapp.exe clone --url=https://github.com/VikashChauhan51/vconsole.git

    ```