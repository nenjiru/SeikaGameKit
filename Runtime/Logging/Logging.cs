using System;
using System.IO;
using UnityEngine;

namespace SeikaGameKit.Logging
{
    /// <summary>
    /// Provides a logging system with multiple log levels and color-coded output for Unity applications.
    /// <example>
    /// <code>
    /// private readonly Logging.Context log = Logging.For(nameof(MyClass));
    /// log.Info("This is an info message", Logging.Color.Cyan);
    /// </code>
    /// </example>
    /// </summary>
    public static class Logging
    {
        /// <summary>
        /// Defines the severity levels for log messages.
        /// </summary>
        public enum LogLevel
        {
            Verbose = 0,
            Info = 1,
            Debug = 2,
            Warning = 3,
            Error = 4,
            Fatal = 5
        }

        /// <summary>
        /// Defines colors for log message formatting in the Unity Editor.
        /// </summary>
        public enum Color
        {
            Default,
            White,
            Red,
            Green,
            Yellow,
            Cyan,
            Magenta,
            Orange,
            Pink
        }

        private static string _logFilePath;
        private static readonly object _lock = new();
        private static LogLevel _minimumLogLevel = LogLevel.Verbose;
        private static bool _isInitialized = false;

        /// <summary>
        /// Gets or sets the minimum log level. Messages below this level will be ignored.
        /// </summary>
        public static LogLevel MinimumLogLevel
        {
            set => _minimumLogLevel = value;
            get => _minimumLogLevel;
        }

        /// <summary>
        /// Creates a logging context for a specific caller.
        /// </summary>
        /// <param name="callerName">The name of the caller to be displayed in log messages.</param>
        /// <returns>A new logging context.</returns>
        public static Context For(string callerName) => new Context(callerName);

        /// <summary>
        /// Represents a logging context that provides methods for logging messages at different levels.
        /// </summary>
        public class Context
        {
            private readonly string _callerName;

            /// <summary>
            /// Gets or sets whether to force logging regardless of the minimum log level.
            /// </summary>
            public bool ForceLog { get; set; }

            /// <summary>
            /// Initializes a new instance of the <see cref="Context"/> class.
            /// </summary>
            /// <param name="callerName">The name of the caller.</param>
            public Context(string callerName)
            {
                _callerName = callerName;
                ForceLog = false;
            }

            /// <summary>
            /// Logs a verbose message.
            /// </summary>
            [HideInCallstack]
            public void Verbose(string message, Color color = Color.Default) => Logging.Log(LogLevel.Verbose, _callerName, message, color, ForceLog);

            /// <summary>
            /// Logs an informational message.
            /// </summary>
            [HideInCallstack]
            public void Info(string message, Color color = Color.Default) => Logging.Log(LogLevel.Info, _callerName, message, color, ForceLog);

            /// <summary>
            /// Logs a debug message.
            /// </summary>
            [HideInCallstack]
            public void Debug(string message, Color color = Color.Default) => Logging.Log(LogLevel.Debug, _callerName, message, color, ForceLog);

            /// <summary>
            /// Logs a warning message.
            /// </summary>
            [HideInCallstack]
            public void Warning(string message, Color color = Color.Yellow) => Logging.Log(LogLevel.Warning, _callerName, message, color, ForceLog);

            /// <summary>
            /// Logs an error message.
            /// </summary>
            [HideInCallstack]
            public void Error(string message, Color color = Color.Red) => Logging.Log(LogLevel.Error, _callerName, message, color, ForceLog);

            /// <summary>
            /// Logs a fatal error message.
            /// </summary>
            [HideInCallstack]
            public void Fatal(string message, Color color = Color.Red) => Logging.Log(LogLevel.Fatal, _callerName, message, color, ForceLog);
        }

        private static void EnsureInitialized()
        {
            if (!_isInitialized)
            {
                lock (_lock)
                {
                    if (!_isInitialized)
                    {
                        var timestamp = DateTime.Now.ToString("yyyyMMdd-HHmmss");
                        var fileName = $"{timestamp}.log";
                        var logDirectory = Path.Combine(Application.persistentDataPath, "Logs");
                        if (!Directory.Exists(logDirectory))
                        {
                            Directory.CreateDirectory(logDirectory);
                        }
                        _logFilePath = Path.Combine(logDirectory, fileName);
                        _isInitialized = true;
                    }
                }
            }
        }

        [HideInCallstack]
        private static void Log(LogLevel level, string callerName, string message, Color color, bool forceLog = false)
        {
            if (!forceLog && level < _minimumLogLevel)
            {
                return;
            }

            var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            var logLevel = level.ToString().ToUpper();

#if UNITY_EDITOR
            var formattedMessage = $"[<color=#AAAAAA><i>{callerName}</i></color>] {WrapWithColor(message, color)}";
            var logMessage = $"[{logLevel}] {formattedMessage}";

            switch (level)
            {
                case LogLevel.Verbose:
                case LogLevel.Debug:
                case LogLevel.Info:
                    UnityEngine.Debug.Log(logMessage);
                    break;
                case LogLevel.Warning:
                    UnityEngine.Debug.LogWarning(logMessage);
                    break;
                case LogLevel.Error:
                case LogLevel.Fatal:
                    UnityEngine.Debug.LogError(logMessage);
                    break;
            }
#else
            EnsureInitialized();
            var logMessage = $"[{timestamp}] [{logLevel}] [{callerName}] {message}";
            WriteToFile(logMessage);
#endif
        }

        private static string WrapWithColor(string message, Color color)
        {
            var colorHex = GetColorHex(color);
            return $"<color={colorHex}>{message}</color>";
        }

        private static string GetColorHex(Color color)
        {
            return color switch
            {
                Color.White => "#FFFFFF",
                Color.Red => "#FF0000",
                Color.Green => "#00FF00",
                Color.Yellow => "#FFFF00",
                Color.Cyan => "#00FFFF",
                Color.Magenta => "#FF00FF",
                Color.Orange => "#FFA500",
                Color.Pink => "#FF69B4",
                _ => "#BBBBBB",
            };
        }

        private static void WriteToFile(string logMessage)
        {
            try
            {
                lock (_lock)
                {
                    File.AppendAllText(_logFilePath, logMessage + Environment.NewLine);
                }
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError($"Failed to write log to file: {ex.Message}");
                UnityEngine.Debug.Log(logMessage);
            }
        }
    }
}