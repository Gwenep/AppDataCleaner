using System;
using System.IO;

namespace AppDataCleaner
{
    /// <summary>
    /// 简单的日志记录工具，记录关键操作到日志文件。
    /// </summary>
    internal static class AppLogger
    {
        private static readonly string LogDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "AppDataCleaner");
        private static readonly string LogFilePath;

        private static readonly object _lock = new();

        static AppLogger()
        {
            try
            {
                Directory.CreateDirectory(LogDir);
            }
            catch
            {
                // 无法创建日志目录时静默失败
            }
            LogFilePath = Path.Combine(LogDir, "cleaner.log");
        }

        public static void Info(string message)
        {
            Write("INFO", message);
        }

        public static void Warn(string message)
        {
            Write("WARN", message);
        }

        public static void Error(string message, Exception? ex = null)
        {
            string detail = ex != null ? $"{message} | {ex.GetType().Name}: {ex.Message}" : message;
            Write("ERROR", detail);
        }

        private static void Write(string level, string message)
        {
            try
            {
                lock (_lock)
                {
                    string line = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level}] {message}";
                    File.AppendAllText(LogFilePath, line + Environment.NewLine, System.Text.Encoding.UTF8);

                    // 日志文件超过 5MB 时截断
                    var fi = new FileInfo(LogFilePath);
                    if (fi.Exists && fi.Length > 5 * 1024 * 1024)
                    {
                        var oldPath = LogFilePath + ".old";
                        File.Delete(oldPath);
                        File.Move(LogFilePath, oldPath);
                    }
                }
            }
            catch
            {
                // 日志写入失败不抛异常
            }
        }
    }
}