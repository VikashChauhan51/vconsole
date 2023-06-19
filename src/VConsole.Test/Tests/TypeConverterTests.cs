using System.Globalization;

namespace VConsole.Test.Tests;


public class TypeConverterTests
{
    enum TestEnum
    {
        ValueA = 1,
        ValueB = 2
    }

    [Flags]
    enum TestFlagEnum
    {
        ValueA = 0x1,
        ValueB = 0x2
    }

    [Theory]
    [MemberData(nameof(ChangeType_scalars_source))]
    public void Test_ChangeType_scalars(string testValue, Type destinationType, object expectedResult)
    {
        var result = Core.TypeConverter.ConvertString(testValue, destinationType, CultureInfo.InvariantCulture);
        Assert.Equal(expectedResult, result);
    }

    [Theory]
    [MemberData(nameof(Overflow_Exception_ChangeType_scalars_source))]
    public void Test_Throws_Overflow_Exception(string testValue, Type destinationType, object expectedResult)
    {
        Action act = () => Core.TypeConverter.ConvertString(testValue, destinationType, CultureInfo.InvariantCulture);
        Assert.Throws<OverflowException>(act);
    }

    [Theory]
    [MemberData(nameof(Format_Exception_ChangeType_scalars_source))]
    public void Test_Format_Exception(string testValue, Type destinationType, object expectedResult)
    {
        Action act = () => Core.TypeConverter.ConvertString(testValue, destinationType, CultureInfo.InvariantCulture);
        Assert.Throws<FormatException>(act);
    }


    [Theory]
    [MemberData(nameof(ChangeType_scalars_culture_source))]
    public void Test_ChangeType_scalars_Culture_Value(string testValue, Type destinationType, CultureInfo culture, object expectedResult)
    {
        var result = Core.TypeConverter.ConvertString(testValue, destinationType, culture);
        Assert.Equal(expectedResult, result);
    }

    public static IEnumerable<object[]> ChangeType_scalars_source
    {
        get
        {
            return new[]
            {
                    new object[] {"1", typeof (int), 1},
                    new object[] {"0", typeof (int), 0},
                    new object[] {"-1", typeof (int), -1},
                    new object[] {int.MaxValue.ToString(), typeof (int), int.MaxValue},
                    new object[] {int.MinValue.ToString(), typeof (int), int.MinValue},

                    new object[] {"1", typeof (uint), (uint) 1},
                    new object[] {uint.MaxValue.ToString(), typeof (uint), uint.MaxValue},
                    new object[] {uint.MinValue.ToString(), typeof (uint), uint.MinValue},

                    new object[] {"true", typeof (bool), true},
                    new object[] {"True", typeof (bool), true},
                    new object[] {"TRUE", typeof (bool), true},
                    new object[] {"false", typeof (bool), false},
                    new object[] {"False", typeof (bool), false},
                    new object[] {"FALSE", typeof (bool), false},

                    new object[] {"10,050.45", typeof (float), 10050.45f},
                    new object[] {"1.0", typeof (float), 1.0f},
                    new object[] {"0.0", typeof (float), 0.0f},
                    new object[] {"-1.0", typeof (float), -1.0f},

                    new object[] {"1.0", typeof (double), 1.0},
                    new object[] {"0.0", typeof (double), 0.0},
                    new object[] {"-1.0", typeof (double), -1.0},

                    new object[] {"1.0", typeof (decimal), 1.0m},
                    new object[] {"0.0", typeof (decimal), 0.0m},
                    new object[] {"-1.0", typeof (decimal), -1.0m},
                    new object[] {"-1.123456", typeof (decimal), -1.123456m},

                    new object[] {"", typeof (string), ""},
                    new object[] {"abcd", typeof (string), "abcd"},

                    new object[] {"ValueA", typeof (TestEnum), TestEnum.ValueA},
                    new object[] {"VALUEA", typeof (TestEnum), TestEnum.ValueA},
                    new object[] {"ValueB", typeof(TestEnum), TestEnum.ValueB},
                    new object[] {((int) TestEnum.ValueA).ToString(), typeof (TestEnum), TestEnum.ValueA},
                    new object[] {((int) TestEnum.ValueB).ToString(), typeof (TestEnum), TestEnum.ValueB},

                    new object[] {"ValueA", typeof (TestFlagEnum), TestFlagEnum.ValueA},
                    new object[] {"VALUEA", typeof (TestFlagEnum), TestFlagEnum.ValueA},
                    new object[] {"ValueB", typeof(TestFlagEnum), TestFlagEnum.ValueB}
                };
        }
    }

    public static IEnumerable<object[]> Format_Exception_ChangeType_scalars_source
    {
        get
        {
            return new[]
            {
                    new object[] {"abcd", typeof (int), null},
                    new object[] {"1.0", typeof (int), null},
                    new object[] {"abcd", typeof (bool), null},
                    new object[] {"0", typeof (bool), null},
                    new object[] {"1", typeof (bool), null},
                    new object[] {"abcd", typeof (float), null},
                    new object[] {"abcd", typeof (double), null},
                    new object[] {"abcd", typeof (decimal), null},
                    new object[] {"false", typeof (int), null},
                    new object[] {"true", typeof (int), null }
                };
        }
    }

    public static IEnumerable<object[]> Overflow_Exception_ChangeType_scalars_source
    {
        get
        {
            return new[]
            {
                    new object[] {((long) int.MaxValue + 1).ToString(), typeof (int), null},
                    new object[] {((long) int.MinValue - 1).ToString(), typeof (int), null},
                    new object[] {((long) uint.MaxValue + 1).ToString(), typeof (uint), null},
                    new object[] {(-1).ToString(), typeof (uint), null}
            };
        }
    }

    public static IEnumerable<object[]> ChangeType_scalars_culture_source
    {
        get
        {
            return new[]
            {
                    new object[] {"1", typeof (int), new CultureInfo("fr-FR"), 1 },
                    new object[] {"0", typeof (int), new CultureInfo("fr-FR"), 0 },
                    new object[] {"-1", typeof (int), new CultureInfo("fr-FR"), -1},
                    new object[] {int.MaxValue.ToString(), typeof (int), new CultureInfo("fr-FR"), int.MaxValue},
                    new object[] {int.MinValue.ToString(), typeof (int), new CultureInfo("fr-FR"), int.MinValue},

                    new object[] {"1", typeof (uint), new CultureInfo("fr-FR"), (uint) 1},
                    new object[] {uint.MaxValue.ToString(), typeof (uint), new CultureInfo("fr-FR"), uint.MaxValue},
                    new object[] {uint.MinValue.ToString(), typeof (uint), new CultureInfo("fr-FR"), uint.MinValue},

                    new object[] {"true", typeof (bool), new CultureInfo("fr-FR"), true },
                    new object[] {"True", typeof (bool), new CultureInfo("fr-FR"), true },
                    new object[] {"TRUE", typeof (bool), new CultureInfo("fr-FR"), true },
                    new object[] {"false", typeof (bool), new CultureInfo("fr-FR"), false },
                    new object[] {"False", typeof (bool), new CultureInfo("fr-FR"), false },
                    new object[] {"FALSE", typeof (bool), new CultureInfo("fr-FR"), false },

                    new object[] {"100 050,45", typeof (float), new CultureInfo("fr-FR"), 100050.45f },
                    new object[] {"1,0", typeof (float), new CultureInfo("fr-FR"), 1.0f },
                    new object[] {"0,0", typeof (float), new CultureInfo("fr-FR"), 0.0f },
                    new object[] {"-1,0", typeof (float), new CultureInfo("fr-FR"), -1.0f},

                    new object[] {"1,0", typeof (double), new CultureInfo("fr-FR"), 1.0 },
                    new object[] {"0,0", typeof (double), new CultureInfo("fr-FR"), 0.0 },
                    new object[] {"-1,0", typeof (double), new CultureInfo("fr-FR"), -1.0},

                    new object[] {"1,0", typeof (decimal), new CultureInfo("fr-FR"), 1.0m },
                    new object[] {"0,0", typeof (decimal), new CultureInfo("fr-FR"), 0.0m },
                    new object[] {"-1,0", typeof (decimal), new CultureInfo("fr-FR"), -1.0m},
                    new object[] {"-1,123456", typeof (decimal), new CultureInfo("fr-FR"), -1.123456m}
                };
        }
    }
}
