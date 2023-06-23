using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace VConsole;
public class ConsoleHost: IConsole
{
    private readonly Type _consoleType = typeof(Console);
    private readonly MethodInfo _consoleWriteMethod;
    private readonly MethodInfo _consoleWriteLineMethod;
    private readonly MethodInfo _consoleReadLineMethod;
    private readonly PropertyInfo _consoleForegroundColorProperty;

    public ConsoleHost()
    {
        _consoleWriteMethod = _consoleType.GetRuntimeMethod("Write", new[] { typeof(string) });
        _consoleWriteLineMethod = _consoleType.GetRuntimeMethod("WriteLine", new[] { typeof(string) });
        _consoleReadLineMethod = _consoleType.GetRuntimeMethod("ReadLine", new Type[] { });
        _consoleForegroundColorProperty = _consoleType.GetRuntimeProperty("ForegroundColor");
    }

    public void WriteMessage(string message)
    {
        _consoleWriteMethod.Invoke(null, new object[] { message });
    }
    public void WriteLine(string line)
    {
        _consoleWriteLineMethod.Invoke(null, new object[] { line });
    }
    public void WriteError(string message)
    {
        var color = _consoleForegroundColorProperty.GetValue(null);
        _consoleForegroundColorProperty.SetValue(null, 12); // red
        _consoleWriteMethod.Invoke(null, new object[] { message });
        _consoleForegroundColorProperty.SetValue(null, color);
    }

    public string ReadValue(string message)
    {
        _consoleWriteMethod.Invoke(null, new object[] { "\n" + message });
        return (string)_consoleReadLineMethod.Invoke(null, new object[] { });
    }

}
