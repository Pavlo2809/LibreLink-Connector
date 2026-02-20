using System.IO;
using System.Text;

namespace LibreLinkConnector.Services
{
    public static class ApiLogger
    {
        private static readonly string LogPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "LibreLinkConnector",
            "api-debug.log"
        );

        private static readonly object _lock = new object();

        static ApiLogger()
        {
            try
            {
                var directory = Path.GetDirectoryName(LogPath);
                if (directory != null && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
            }
            catch { }
        }

        public static void LogRequest(string method, string url, IEnumerable<string> headers, string? body = null)
        {
            try
            {
                lock (_lock)
                {
                    var sb = new StringBuilder();
                    sb.AppendLine($"\n========== REQUEST at {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} ==========");
                    sb.AppendLine($"{method} {url}");
                    sb.AppendLine("\nHeaders:");
                    foreach (var header in headers)
                    {
                        sb.AppendLine($"  {header}");
                    }
                    if (!string.IsNullOrEmpty(body))
                    {
                        sb.AppendLine("\nBody:");
                        sb.AppendLine(body);
                    }
                    sb.AppendLine();

                    File.AppendAllText(LogPath, sb.ToString());
                }
            }
            catch { }
        }

        public static void LogResponse(int statusCode, string statusText, IEnumerable<string> headers, string body)
        {
            try
            {
                lock (_lock)
                {
                    var sb = new StringBuilder();
                    sb.AppendLine($"========== RESPONSE at {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} ==========");
                    sb.AppendLine($"Status: {statusCode} {statusText}");
                    sb.AppendLine("\nHeaders:");
                    foreach (var header in headers)
                    {
                        sb.AppendLine($"  {header}");
                    }
                    sb.AppendLine("\nBody:");
                    sb.AppendLine(body);
                    sb.AppendLine("\n" + new string('=', 80) + "\n");

                    File.AppendAllText(LogPath, sb.ToString());
                }
            }
            catch { }
        }

        public static void LogError(string message, Exception? ex = null)
        {
            try
            {
                lock (_lock)
                {
                    var sb = new StringBuilder();
                    sb.AppendLine($"\n========== ERROR at {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} ==========");
                    sb.AppendLine(message);
                    if (ex != null)
                    {
                        sb.AppendLine($"Exception: {ex.GetType().Name}");
                        sb.AppendLine($"Message: {ex.Message}");
                        sb.AppendLine($"StackTrace:\n{ex.StackTrace}");
                    }
                    sb.AppendLine("\n" + new string('=', 80) + "\n");

                    File.AppendAllText(LogPath, sb.ToString());
                }
            }
            catch { }
        }

        public static string GetLogPath() => LogPath;

        public static void ClearLog()
        {
            try
            {
                lock (_lock)
                {
                    if (File.Exists(LogPath))
                    {
                        File.Delete(LogPath);
                    }
                    File.WriteAllText(LogPath, $"API Debug Log - Started at {DateTime.Now:yyyy-MM-dd HH:mm:ss}\n");
                }
            }
            catch { }
        }
    }
}
