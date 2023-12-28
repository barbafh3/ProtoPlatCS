using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace ProtoPlat;

public enum LogLevel
{
    INFO,
    WARNING,
    ERROR
}

public static class GameLogger
{
    public static void Log(LogLevel logLevel, string message, bool printTimestamp = false)
    {
        var timestamp = DateTime.UtcNow;
        var caller = new StackTrace().GetFrame(1)?.GetMethod();
        var callerClass = caller?.ReflectedType?.Name;
        var callerMethod = caller?.Name;

        var callerString = "";
        if (!String.IsNullOrEmpty(callerClass))
        {
            callerString += $"[{callerClass}";
            if (String.IsNullOrEmpty(callerMethod))
                callerString += "]";
            else
                callerString += $".{callerMethod}]";
        }
        else if (!String.IsNullOrEmpty(callerMethod))
            callerString += $"[{callerMethod}]";


        switch (logLevel)
        {
            case LogLevel.INFO:
            {
                Console.ForegroundColor = ConsoleColor.White;
                break;
            }
            case LogLevel.WARNING:
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                break;
            }
            case LogLevel.ERROR:
            {
                Console.ForegroundColor = ConsoleColor.Red;
                break;
            }
        }
        
        
        if (printTimestamp)
            Console.WriteLine($"[LOG][{timestamp}][{logLevel}]{callerString} {message}");
        else
            Console.WriteLine($"[LOG][{logLevel}]{callerString} {message}");
        Console.ForegroundColor = ConsoleColor.White;
    }
}