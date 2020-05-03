namespace Comparator.Utils.Logger {
    public interface ILoggerManager {
        void LogDebug(string message);
        void LogError(string message);
        void LogInfo(string message);
        void LogWarning(string message);
    }
}