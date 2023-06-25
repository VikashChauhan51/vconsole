namespace VConsole.Test.Fakes;

public enum FakeEnum
{
    None,
    First,
    Second,
    Third
}

[Verb("fake", new string[] { "faky" }, "fake command", typeof(FakeCommand))]
public class FakeCommand : ICommand
{
    [Option('i', "int")]
    public int FakeInt { get; set; }
    [Option('s', "string", Default = "")]
    public string FakeString { get; set; }

    [Option('d', "date")]
    public DateTime FakeDate { get; set; }
    [Option('t', "time")]
    public TimeSpan FakeTime { get; set; }

    [Option('o', "offset")]
    public DateTimeOffset? FakeDateOffset { get; set; }

    [Option('m', "timeonly")]
    public TimeOnly FakeTimeOnly { get; set; }

    public Task Execute()
    {
        Console.WriteLine("executed");
        return Task.CompletedTask;
    }
}




public class FakeLogger
{
    public void Log(string message)
    {
        Console.WriteLine(message);
    }
}


[Verb("fake-dependency",null, "fake command with dependency", typeof(FakeCommand))]
public class FakeDependencyCommand : ICommand
{
    private readonly FakeLogger logger;


    [Option('m', "message", Default = "")]
    public string Message { get; set; }
    public FakeDependencyCommand(FakeLogger logger)
    {
        this.logger = logger;
    }

    public Task Execute()
    {
        logger.Log(Message);
        return Task.CompletedTask;
    }
}

public enum Level
{
    Normal = 0,
    Error = 1,
    Warning = 2,
    Info = 3,
    Verbose = 4,

}

[Verb("fake-enum", null, "fake enum command", typeof(FakeEnumCommand))]
public class FakeEnumCommand : ICommand
{
    [Option('e', "enum")]
    public Level Level { get; set; }

    [Option('g', "guid")]
    public Guid Id { get; set; }
    public Task Execute()
    {
        Console.WriteLine($"{Id} {Level}");
        return Task.CompletedTask;
    }
}

[Verb("fake-default",helpText:"Default command parameters name")]
public class FakeDefaultCommand : ICommand
{
    [Option]
    public string Title { get; set; } = string.Empty;

    [Option]
    public int Id { get; set; }
    public Task Execute()
    {
        Console.WriteLine($"{Title} :{Id}");
        return Task.CompletedTask;
    }
}
