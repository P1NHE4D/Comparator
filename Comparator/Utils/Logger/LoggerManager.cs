using Microsoft.Extensions.Hosting;
using NLog;

namespace Comparator.Utils.Logger {
    public class LoggerManager : ILoggerManager {
        private readonly ILogger _logger;

        public LoggerManager(IHostEnvironment env) {
            var name = env.IsDevelopment() ? "Debug" : "Prod";
            _logger = LogManager.GetLogger(name);
        }

        /// <summary>
        /// Log a debug message
        /// </summary>
        /// <param name="message">message to be logged</param>
        public void LogDebug(string message) {
            _logger.Debug(message);
        }

        /// <summary>
        /// Log an error message
        /// </summary>
        /// <param name="message">message to be logged</param>
        public void LogError(string message) {
            _logger.Error(message);
        }

        /// <summary>
        /// Log an info message
        /// </summary>
        /// <param name="message">message to be logged</param>
        public void LogInfo(string message) {
            _logger.Info(message);
        }

        /// <summary>
        /// Log a warning message
        /// </summary>
        /// <param name="message">message to be logged</param>
        public void LogWarning(string message) {
            _logger.Warn(message);
        }
    }
}