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
    public static void Test_Parser_With_DependencyResolver()
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

        parser
            .RegisterCommand<FakeDependencyCommand>()
            .ParseArguments(args);
    }


    [Theory]
    [MemberData(nameof(Invalid_Length_Agrs))]
    public static void Test_Parser_GetValue_Argument_Exception_With_Invalid_Length_Agrs(string[] args)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Parser.Default
           .ClearCommands()
           .RegisterCommand(typeof(FakeCommand))
           .ParseArguments(args));
    }

    [Theory]
    [MemberData(nameof(Valid_Agrs))]
    public static void Test_Parser_GetValue_With_Valid_Agrs(string[] args)
    {
        var exception = Record.Exception(() => Parser.Default
            .ClearCommands()
            .RegisterCommand<FakeCommand>()
            .ParseArguments(args));

        //Assert
        Assert.Null(exception);
    }

    [Fact]
    public static void Test_Parser_GetValue_With_Valid_Agrs_And_Properties()
    {
        var culture = Thread.CurrentThread.CurrentCulture;
        var setting = new ParserSettings()
        {
            ParsingCulture = culture
        };
        var parser = new Parser(setting);

        var date = DateTime.Now;
        var dateOffset = DateTimeOffset.Now;
        var args = new string[]
        {
            "faky",
            $"--int=123",
            "--string=hello",
            $"--date={date.ToString(culture)}",
            $"--time={TimeSpan.FromSeconds(300)}",
            $"--offset={dateOffset.ToString(culture)}",
             $"--timeonly={TimeOnly.FromTimeSpan(TimeSpan.FromSeconds(20)).ToString(culture)}"
        };

        var exception = Record.Exception(() => parser
           .RegisterCommand<FakeCommand>()
           .ParseArguments(args));

        //Assert
        Assert.Null(exception);

    }


    [Fact]
    public static void Test_Parser_GetValue_With_Valid_Enum_Agrs()
    {
        var culture = Thread.CurrentThread.CurrentCulture;
        var setting = new ParserSettings()
        {
            ParsingCulture = culture
        };
        var parser = new Parser(setting);

        var args = new string[]
        {
            "fake-enum",
            $"--guid={Guid.NewGuid()}",
            $"--enum={Level.Warning}"
        };

        var exception = Record.Exception(() => parser
           .RegisterCommand<FakeEnumCommand>()
           .ParseArguments(args));

        //Assert
        Assert.Null(exception);

    }

    [Fact]
    public static void Test_Parser_GetValue_By_Parameter_Name()
    {
        var culture = Thread.CurrentThread.CurrentCulture;
        var setting = new ParserSettings()
        {
            ParsingCulture = culture
        };
        var parser = new Parser(setting);

        var args = new string[]
        {
            "fake-default",
            $"--Title={Guid.NewGuid()}",
            $"--Id=100"
        };

        var exception = Record.Exception(() => parser
           .RegisterCommand<FakeDefaultCommand>()
           .ParseArguments(args));

        //Assert
        Assert.Null(exception);

    }

    public static IEnumerable<object[]> Invalid_Length_Agrs
    {
        get
        {
            return new[]
            {
                    new object[] { new string[] {"fake","" } },
                    new object[] { new string[] { "fake", "abcd" }},
                    new object[] { new string[] { "fake", "--url>'testing'" }},
            };
        }
    }

    public static IEnumerable<object[]> Valid_Agrs
    {
        get
        {
            return new[]
            {
                    new object[] { new string[] { "fake", "url=testing" } },
                    new object[] { new string[] { "fake", "count=123" }},
                    new object[] { new string[] { "fake", "--url=testing" }},
            };
        }
    }
}
