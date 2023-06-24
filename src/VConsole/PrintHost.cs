
using System;

namespace VConsole;
internal class PrintHost : IConsole
{
    public string ReadValue(string message)
    {
        Console.Write(message);
        return Console.ReadLine()!;
    }

    public void WriteError(string message)
    {
        var color = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write(message);
        Console.ForegroundColor = color;
    }

    public void WriteLine(string line)
    {
        Console.WriteLine(line);
    }

    public void WriteMessage(string message)
    {
        Console.Write(message);
    }
}
