namespace VConsole.Test.Tests;

public class VerbAttributeTest
{

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("  ")]
    public static void Test_Verb_Name_Text_Argument_Exception(string name)
    {
        Assert.Throws<ArgumentException>(() => new VerbAttribute(name));
    }

    [Theory]
    [InlineData("first")]
    [InlineData("second name")]
    public static void Test_Verb_Name_Text(string name)
    {
        //Act
        var exception = Record.Exception(() => new VerbAttribute(name));

        //Assert
        Assert.Null(exception);
    }
}
