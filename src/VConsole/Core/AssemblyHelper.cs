using System.Reflection;

namespace VConsole.Core;
internal static class AssemblyHelper
{
    private const string @default = "unknown";
    public static void PrintInfo(IConsole console)
    {
        var assembly = Assembly.GetEntryAssembly();
        var nameInfo = assembly?.GetName();
        var version = nameInfo?.Version?.ToString() ?? @default;
        var name = nameInfo?.Name ?? @default;
        console.WriteLine(name);
        console.WriteLine("");
        console.WriteLine($"version: {version}");
    }
}
