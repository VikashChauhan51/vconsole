# VConsole
VConsole is a .NET library to parse command line arguments and execute commands.

The **[VConsole](https://www.nuget.org/packages/VConsole)** command Line Parser Library offers CLR applications a clean and concise API for manipulating command line arguments and related tasks, such as defining switches, options and verb commands. It allows you to display a help screen with a high degree of customization and a simple way to report syntax errors to the end user.

```cmd
 dotnet add package VConsole
```
or

```cmd
 NuGet\Install-Package VConsole
```

## At a glance:
- Compatible with **.NET Core 6+**.
- Doesn't depend on other packages (No dependencies beyond standard base libraries).
- One line parsing using default singleton:` VConsole.Parser.Default.ParseArguments(...)` and multiples overload methods.
- Map to scalar types, including `Enums`, `Guid`,`datetimeoffset` and **Nullable** scalar types, `Enums`,`datetimeoffset` and `Guid`.
- Automatically ignore unused and additional provided parameters.
- Automatically map parameter if value is:(with long name) `--url=value`, `-url=value`, `url=value` and (with short name) `--u=value`, `-u=value`, `u=value`.
- Default `help` command: `myapp.exe help -c=command` or `myapp.exe --help`.
- Default `version` command: `myapp.exe version` or `myapp.exe --version`.
- Interactive mode support.
- Support custom ***DependencyInjection*** to resolve the command dependencies with the help of `IDependencyResolver` interface.
- Any **Culture** support as per your requirment. Default parser has ***InvariantCulture***.
- Support custom parameter value separator. Default parser has `=` separator i.e.: `--parm=value`.


## Quick Start Example
- Create a class to define valid `command` with `varb` and `options` **attrbutes** to receive the parsed options.
- Register commands using `RegisterCommand` or `RegisterCommandsFromAssembly` methods.
- Call `ParseArguments` with the `args` string array.

Example:

``` C#
using VConsole;

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

```cmd
# Build your application and run it like this:
myapp.exe clone --url=https://github.com/VikashChauhan51/vconsole.git

 ```


## Dependency Resolver Example:

Here we took an example with Microsoft ***Dependency Injection***, but you can use any one you prefer. Please add following nuget packages before to proceed:
- VConsole
- Microsoft.Extensions.DependencyInjection
- Microsoft.Extensions.Logging.Console

```C#
// Crate a fake dependency service for command

public interface IFooService
{
    void DoThing(string message);
}

public class FooService : IFooService
{
    private readonly ILogger<FooService> logger;
    public FooService(ILoggerFactory loggerFactory)
    {
        logger = loggerFactory.CreateLogger<FooService>();
    }

    public void DoThing(string message)
    {
        logger.LogInformation($"Doing the thing {message}");
    }
}

```

```C#
// Create a command with dependency

[Verb("clone", HelpText = "Clone a repository into a new directory.")]
public class Clone : ICommand
{
    [Option('u', "url", Required = true, HelpText = "Cloud repository URL.")]
    public string URL { get; set; } = string.Empty;

    private readonly IFooService fooService;
    private readonly ILogger<Clone> logger;
    public Clone(IFooService fooService, ILogger<Clone> logger)
    {
        this.fooService = fooService;
        this.logger = logger;
    }
    public void Execute()
    {
        fooService.DoThing("Pulling...");
        logger.LogInformation($"Cloning a repository: {URL}");
    }
}


```

``` C#
// Create a service resolver

public class DependencyResolver : IDependencyResolver
{
    private readonly ServiceProvider serviceProvider;

    public DependencyResolver(ServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }
    public object GetService(Type serviceType) => serviceProvider.GetService(serviceType) ?? throw new ArgumentOutOfRangeException(nameof(serviceType));

}

```

``` C#
// create DI container and parser. (Program.cs)

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using VConsole;


//setup our DI
var serviceProvider = new ServiceCollection()
    .AddLogging(x => x.AddConsole())
    .AddSingleton<IFooService, FooService>()
    .BuildServiceProvider();

//setup dependency resolver
var serviceResolver = new DependencyResolver(serviceProvider);

//create parser
var parser = new Parser(serviceResolver);

// configure commands
parser
    .RegisterCommand<Clone>()
    .ParseArguments(args);

```

```cmd
# Build your application and run it like this:

myapp.exe clone --url=https://github.com/VikashChauhan51/vconsole.git

```

## Culture Example:
Default parser has ***InvariantCulture*** to parse command parameters values:

``` C#
using VConsole;

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

//create parser settings
var settings = new ParserSettings
    {
        // set current culture instead of Invariant.
        ParsingCulture = Thread.CurrentThread.CurrentCulture
    };

//create parser with settings
var parser = new Parser(settings);

// configure commands
parser
    .RegisterCommand<Clone>()
    .ParseArguments(args);

```

```cmd
# Build your application and run it like this:

myapp.exe clone --url=https://github.com/VikashChauhan51/vconsole.git

```

## Interactive Mode Example:
Default parser has ***InteractiveMode*** off:

``` C#
using VConsole;

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

//create parser settings
var settings = new ParserSettings
{
    InteractiveMode = true
};

//create parser with settings
var parser = new Parser(settings);

// configure commands
parser
    .RegisterCommand<Clone>()
    .ParseArguments(args);

```

```cmd
# Build your application and run it without any arguments:

myapp.exe

```

## Custom Separator Example:
Default parser has `=` value for ***Separator***.

``` C#
using VConsole;

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

//create parser settings
var settings = new ParserSettings
{
    Separator = ':'
};

//create parser with settings
var parser = new Parser(settings);

// configure commands
parser
    .RegisterCommand<Clone>()
    .ParseArguments(args);

```

```cmd
# Build your application and run it like this:

myapp.exe clone --url:https://github.com/VikashChauhan51/vconsole.git

```

## Default Help Command Example:

``` C#
using VConsole;

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

// configure commands
Parser.Default
    .RegisterCommand<Clone>()
    .ParseArguments(args);

```

```cmd
# Build your application and run it like this:

myapp.exe help --command=clone
or
myapp.exe help -c=clone
```