
namespace VConsole.Test.Tests;

public class OptionAttributeTest
{

    [Fact]
    public static void Test_Option_LongName_Argument_Exception()
    {
        Assert.Throws<ArgumentNullException>(() => new OptionAttribute(null));
    }

    [Fact]
    public static void Test_Option_LongName_Not_Argument_Exception()
    {
        //Act
        var exception = Record.Exception(() => new OptionAttribute("first"));

        //Assert
        Assert.Null(exception);
    }

    [Fact]
    public static void Test_Option_ShortName_Not_Argument_Exception()
    {
        //Act
        var exception = Record.Exception(() => new OptionAttribute('c'));

        //Assert
        Assert.Null(exception);
    }

    [Fact]
    public static void Test_Option_With_LongName_And_ShortName()
    {
        //Act
        var exception = Record.Exception(() => new OptionAttribute('f', "first"));

        //Assert
        Assert.Null(exception);
    }

    [Fact]
    public static void Test_Option_Default_Not_Argument_Exception()
    {
        //Act
        var exception = Record.Exception(() => new OptionAttribute());

        //Assert
        Assert.Null(exception);
    }
}
