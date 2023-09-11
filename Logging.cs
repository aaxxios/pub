
using System.Runtime.CompilerServices;

namespace PublisherBot;

internal class Logging
{

    public static void LogInfo(string message, [CallerMemberName] string caller = "")
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"[INFO] {caller} :: {message}");
        Console.ResetColor();
    }

    public static void LogError(string message, [CallerMemberName] string caller = "")
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"[ERROR] {caller} :: {message}");
        Console.ResetColor();
    }

    public static void LogWarning(string message, [CallerMemberName] string caller = "")
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"[WARNING] {caller} :: {message}");
        Console.ResetColor();
    }
}