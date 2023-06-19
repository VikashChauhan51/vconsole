using System.Reflection;
using VConsole.Test.Fakes;

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
    public static void Test_Option_GetValue_Argument_Exception_With_Null_Property()
    {
        var setting = new ParserSettings();
        var option = new OptionAttribute(string.Empty);

        Assert.Throws<ArgumentNullException>(() => option.GetValue(null, setting.Separator, new string[0], setting.NameComparer, setting.ParsingCulture));
    }

    [Fact]
    public static void Test_Option_GetValue_Argument_Exception_With_Null_Args()
    {
        var setting = new ParserSettings();
        var option = new OptionAttribute(string.Empty);
        var type = typeof(OptionAttribute);
        var property = type.GetProperties().First();
        Assert.Throws<ArgumentNullException>(() => option.GetValue(property, setting.Separator, null, setting.NameComparer, setting.ParsingCulture));
    }

    [Fact]
    public static void Test_Option_GetValue_Argument_Exception_With_Null_NameComparer()
    {
        var setting = new ParserSettings();
        var option = new OptionAttribute(string.Empty);
        var type = typeof(OptionAttribute);
        var property = type.GetProperties().First();
        Assert.Throws<ArgumentNullException>(() => option.GetValue(property, setting.Separator, new string[0], null, setting.ParsingCulture));
    }

    [Fact]
    public static void Test_Option_GetValue_Argument_Exception_With_Null_ParsingCulture()
    {
        var setting = new ParserSettings();
        var option = new OptionAttribute(string.Empty);
        var type = typeof(OptionAttribute);
        var property = type.GetProperties().First();
        Assert.Throws<ArgumentNullException>(() => option.GetValue(property, setting.Separator, new string[0], setting.NameComparer, null));
    }

    [Fact]
    public static void Test_Option_GetValue_Argument_Exception_With_Null_EmptyNames()
    {
        var setting = new ParserSettings();
        var option = new OptionAttribute(string.Empty);
        var type = typeof(OptionAttribute);
        var property = type.GetProperties().First();
        Assert.Throws<ArgumentException>(() => option.GetValue(property, setting.Separator, new string[0], setting.NameComparer, setting.ParsingCulture));
    }


    [Theory]
    [MemberData(nameof(Invalid_Length_Agrs))]
    public static void Test_Option_GetValue_Argument_Exception_With_Invalid_Length_Agrs(string[] args)
    {
        var setting = new ParserSettings();
        var option = new OptionAttribute(Guid.NewGuid().ToString());
        var type = typeof(OptionAttribute);
        var property = type.GetProperties().First();
        Assert.Throws<ArgumentOutOfRangeException>(() => option.GetValue(property, setting.Separator, args, setting.NameComparer, setting.ParsingCulture));
    }

    [Theory]
    [MemberData(nameof(Valid_Agrs))]
    public static void Test_Option_GetValue_With_Valid_Agrs(string[] args, string name, string value)
    {
        var setting = new ParserSettings();
        var option = new OptionAttribute(name);
        var type = typeof(OptionAttribute);
        var property = type.GetProperty("LongName");
        var result = option.GetValue(property, setting.Separator, args, setting.NameComparer, setting.ParsingCulture);
        Assert.Equal(value, result);
    }

    [Fact]
    public static void Test_Option_GetValue_With_Valid_Agrs_And_Properties()
    {
        var culture = Thread.CurrentThread.CurrentCulture;
        var setting = new ParserSettings()
        {
            ParsingCulture = culture
        };
        var type = typeof(FakeCommand);
        var properties = type.GetProperties();
        var date = DateTime.Now;
        var dateOffset = DateTimeOffset.Now;
        var args = new string[]
        {
            $"--int=123",
            "--string=hello",
            $"--date={date.ToString(culture)}",
            $"--time={TimeSpan.FromSeconds(300)}",
            $"--offset={dateOffset.ToString(culture)}",
             $"--timeonly={TimeOnly.FromTimeSpan(TimeSpan.FromSeconds(20)).ToString(culture)}"
        };

        foreach (var property in properties)
        {
            var option = property.GetCustomAttribute<OptionAttribute>();
            if (option is not null)
            {
                var result = option.GetValue(property, setting.Separator, args, setting.NameComparer, setting.ParsingCulture);
                Assert.NotNull(result);
            }
        }

    }

    public static IEnumerable<object[]> Invalid_Length_Agrs
    {
        get
        {
            return new[]
            {
                    new object[] { new string[] {"" } },
                    new object[] { new string[] {"abcd" }},
                    new object[] { new string[] {"--url>'testing'" }},
            };
        }
    }

    public static IEnumerable<object[]> Valid_Agrs
    {
        get
        {
            return new[]
            {
                    new object[] { new string[] { "url=testing" },"url","testing" },
                    new object[] { new string[] {"count=123" },"count", "123" },
                    new object[] { new string[] {"--url=testing" }, "url" ,"testing" },
            };
        }
    }
}
