using System.Globalization;
using System.Reflection;
using VConsole.Test.Fakes;

namespace VConsole.Test.Tests;

public class ParserTests
{
    [Fact]
    public static void Test_Default_Parser_With_Valid_Command_Args()
    {
        var culture = CultureInfo.InvariantCulture;
        var date = DateTime.Now;
        var dateOffset = DateTimeOffset.Now;
        var args = new string[]
        {
            $"fake",
            $"--int=123",
            "--string=hello",
            $"--date={date.ToString(culture)}",
            $"--time={TimeSpan.FromSeconds(300)}",
            $"--offset={dateOffset.ToString(culture)}",
             $"--timeonly={TimeOnly.FromTimeSpan(TimeSpan.FromSeconds(20)).ToString(culture)}"
        };

        var exception = Record.Exception(() => Parser.Default
            .ClearCommands()
            .RegisterCommand<FakeCommand>()
            .ParseArguments(args));

        //Assert
        Assert.Null(exception);
    }

    [Fact]
    public static void Test_Default_Parser_With_Aliase_Command_Args()
    {
        var culture = CultureInfo.InvariantCulture;
        var date = DateTime.Now;
        var dateOffset = DateTimeOffset.Now;
        var args = new string[]
        {
            $"faky",
            $"--int=123",
            "--string=hello",
            $"--date={date.ToString(culture)}",
            $"--time={TimeSpan.FromSeconds(300)}",
            $"--offset={dateOffset.ToString(culture)}",
             $"--timeonly={TimeOnly.FromTimeSpan(TimeSpan.FromSeconds(20)).ToString(culture)}"
        };

        var exception = Record.Exception(() => Parser.Default
            .ClearCommands()
            .RegisterCommand<FakeCommand>()
            .ParseArguments(args));

        //Assert
        Assert.Null(exception);
    }

    [Fact]
    public static void Test_Default_Parser_With_Assembly_Commands()
    {
        var culture = CultureInfo.InvariantCulture;
        var date = DateTime.Now;
        var dateOffset = DateTimeOffset.Now;
        var args = new string[]
        {
            $"fake",
            $"--int=123",
            "--string=hello",
            $"--date={date.ToString(culture)}",
            $"--time={TimeSpan.FromSeconds(300)}",
            $"--offset={dateOffset.ToString(culture)}",
             $"--timeonly={TimeOnly.FromTimeSpan(TimeSpan.FromSeconds(20)).ToString(culture)}"
        };

        var exception = Record.Exception(() => Parser.Default
            .ClearCommands()
            .RegisterCommandsFromAssembly(Assembly.GetExecutingAssembly())
            .ParseArguments(args));

        //Assert
        Assert.Null(exception);
    }

    [Fact]
    public static void Test_Default_Parser_With_Command_Type()
    {
        var culture = CultureInfo.InvariantCulture;
        var date = DateTime.Now;
        var dateOffset = DateTimeOffset.Now;
        var args = new string[]
        {
            $"fake",
            $"--int=123",
            "--string=hello",
            $"--date={date.ToString(culture)}",
            $"--time={TimeSpan.FromSeconds(300)}",
            $"--offset={dateOffset.ToString(culture)}",
             $"--timeonly={TimeOnly.FromTimeSpan(TimeSpan.FromSeconds(20)).ToString(culture)}"
        };

        var exception = Record.Exception(() => Parser.Default
            .ClearCommands()
            .RegisterCommand(typeof(FakeCommand))
            .ParseArguments(args));

        //Assert
        Assert.Null(exception);
    }


    [Fact]
    public static void Test_Default_Parser_With_Null_Command()
    { 
        Assert.Throws<ArgumentNullException>(() => Parser.Default
            .ClearCommands()
            .RegisterCommand(null)
            .ParseArguments(new string[0]));
    }

    [Fact]
    public static void Test_Default_Parser_With_Invalid_Command()
    {
        Assert.Throws<InvalidOperationException>(() => Parser.Default
            .ClearCommands()
            .RegisterCommand(typeof(string))
            .ParseArguments(new string[0]));
    }

    [Fact]
    public static void Test_Default_Parser_With_Duplicate_Command()
    {
        Assert.Throws<InvalidOperationException>(() => Parser.Default
            .ClearCommands()
            .RegisterCommand(typeof(FakeCommand))
            .RegisterCommand(typeof(FakeCommand))
            .ParseArguments(new string[0]));
    }

    [Fact]
    public static void Test_Default_Parser_Without_Command()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Parser.Default
            .ClearCommands()
            .ParseArguments(new string[0]));
    }

    [Fact]
    public static void Test_Default_Parser_Without_Args()
    {
        Assert.Throws<ArgumentNullException>(() => Parser.Default
            .ClearCommands()
            .RegisterCommand(typeof(FakeCommand))
            .ParseArguments(null));
    }

    [Fact]
    public static void Test__Parser_With_DependencyResolver()
    {
        var mockDependencyResolver = new Mock<IDependencyResolver>();

        mockDependencyResolver
            .Setup(x => x.GetService(typeof(FakeLogger)))
            .Returns(new FakeLogger());
        
        var parser = new Parser(mockDependencyResolver.Object);
        var args = new string[]
        {
            $"fake-dependency",
            $"-m=testing message"
        };

        parser.ClearCommands()
            .RegisterCommand<FakeDependencyCommand>()
            .ParseArguments(args);
    }
}
