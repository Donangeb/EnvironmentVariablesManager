using Microsoft.Extensions.Logging;
using System.IO;

namespace EnvironmentVariablesManager.Logging
{
    public class FileLogger : ILogger
    {
        private readonly string _categoryName;
        private readonly string _filePath;

        public FileLogger(string categoryName, string filePath)
        {
            _categoryName = categoryName;
            _filePath = filePath;
        }

        public IDisposable BeginScope<TState>(TState state) where TState : notnull => null!;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
                return;

            var message = formatter(state, exception);
            var logMessage = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{logLevel}] {_categoryName}: {message}";

            if (exception != null)
            {
                logMessage += $"\nException: {exception}";
            }

            WriteToFile(logMessage);
        }

        private void WriteToFile(string message)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(_filePath));
                File.AppendAllText(_filePath, message + Environment.NewLine);
            }
            catch
            {
                // Если не удалось записать в файл, игнорируем ошибку
            }
        }
    }

    public class FileLoggerProvider : ILoggerProvider
    {
        private readonly string _filePath;

        public FileLoggerProvider(string filePath)
        {
            _filePath = filePath;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new FileLogger(categoryName, _filePath);
        }

        public void Dispose() { }
    }
}