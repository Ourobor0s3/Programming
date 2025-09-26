public static class SimpleLogger
{
    private static readonly object _lock = new();
    private const string LogDir = "logs";
    private static string LogFile => Path.Combine(LogDir, $"app-{DateTime.Now:yyyy-MM-dd}.log");

    static SimpleLogger()
    {
        Directory.CreateDirectory(LogDir);
    }

    private static void Log(string message, string level = "INFO")
    {
        var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        var logLine = $"[{timestamp}] {level}: {message}";

        lock (_lock)
        {
            Console.WriteLine(logLine);
            File.AppendAllLines(LogFile, new[] { logLine });
        }
    }

    public static void Info(string message) => Log(message, "INFO");
    public static void Warn(string message) => Log(message, "WARN");
    public static void Error(string message) => Log(message, "ERROR");
}