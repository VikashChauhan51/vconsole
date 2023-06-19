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

    public void Execute()
    {
        Console.WriteLine("executed");
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

    public void Execute()
    {
        logger.Log(Message);
    }
}