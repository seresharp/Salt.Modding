using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace Modding
{
    public static class Logger
    {
        private const string LOG_FOLDER = "Mod Logs";

        private static readonly StreamWriter writer;
        public static LogLevel LoggingLevel { get; internal set; }

        static Logger()
        {
            if (!Directory.Exists(LOG_FOLDER))
            {
                Directory.CreateDirectory(LOG_FOLDER);
            }

            string dateTime = DateTime.UtcNow.ToString("MM dd yyyy (HH mm ss)", CultureInfo.InvariantCulture);
            FileStream stream = new FileStream($"{LOG_FOLDER}{Path.DirectorySeparatorChar}{dateTime}.txt", FileMode.Create, FileAccess.Write);
            writer = new StreamWriter(stream, Encoding.UTF8);
            writer.AutoFlush = true;

            LoggingLevel = LogLevel.Debug;

            //This gets logged regardless of any command line arguments but I don't care enough to fix it
            LogDebug("[API] Logger initialized");
        }

        public static void Log(string msg, LogLevel level = LogLevel.Info)
        {
            if (level == LogLevel.None)
            {
                throw new ArgumentException("LogLevel.None is not valid in logging calls");
            }

            //null string normally translates to "", but "null" looks better in logs
            if (msg == null)
            {
                msg = "null";
            }

            if (LoggingLevel <= level)
            {
                string logLevel = LogLevelToString(level);
                string time = DateTime.UtcNow.ToString("HH:mm:ss", CultureInfo.InvariantCulture);
                Write($"[{logLevel}][{time}]{msg}");
            }
        }

        public static void Log(object msg, LogLevel level = LogLevel.Info) => Log(msg?.ToString(), level);

        public static void LogDebug(string msg) => Log(msg, LogLevel.Debug);
        public static void LogDebug(object msg) => LogDebug(msg?.ToString());

        public static void LogWarn(string msg) => Log(msg, LogLevel.Warn);
        public static void LogWarn(object msg) => LogWarn(msg?.ToString());

        public static void LogError(string msg) => Log(msg, LogLevel.Error);
        public static void LogError(object msg) => LogError(msg?.ToString());
        
        private static void Write(string msg)
        {
            if (!msg.EndsWith(Environment.NewLine))
            {
                msg += Environment.NewLine;
            }

            //Lock for thread safety
            lock (writer)
            {
                writer.Write(msg);
            }
        }

        //this should be faster than level.ToString().ToUpper();
        private static string LogLevelToString(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Debug:
                    return "DEBUG";
                case LogLevel.Info:
                    return "INFO";
                case LogLevel.Warn:
                    return "WARN";
                case LogLevel.Error:
                    return "ERROR";
                default:
                    return "NONE";
            }
        }
    }

    public enum LogLevel
    {
        Debug,
        Info,
        Warn,
        Error,
        None
    }
}
