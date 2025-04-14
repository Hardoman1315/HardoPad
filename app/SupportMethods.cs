using System;
using System.IO;


namespace app;

public static class SupportMethods
{
    public static void LogData(string logLevel, string traceID, string message, bool rewrite = false)
    {
        var log = $"[{DateTime.Now}][{logLevel}]({traceID}) {message}";
        if (!rewrite) { File.AppendAllText("logs.log", $"{log}\n"); }
        else { File.WriteAllText("logs.log", $"{log}\n"); }
    }
}
