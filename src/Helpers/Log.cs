using System.Text;
using BepInEx.Logging;

namespace AgriCore.Helpers;

/// <summary>
/// Class helping for logging stuff
/// </summary>
internal static class Log
{
    private static ManualLogSource? _logger;

    /// <summary>
    /// Assigns the current logger to the given logger
    /// </summary>
    public static void SetLogger(ManualLogSource? logger) => _logger = logger;

    private static void LogSelf(object?[] data, LogLevel level)
    {
        if (_logger == null)
            return;

        var message = new StringBuilder();

        for (var i = 0; i < data.Length; i++)
        {
            var content = data[i] ?? "null";

            message.Append(content);

            if (i < data.Length - 1)
                message.Append(" ");
        }
        
        _logger.Log(level, message.ToString());
    }

    /// <inheritdoc cref="BepInEx.Logging.ManualLogSource.LogDebug"/>
    public static void Debug(params object?[] data) => LogSelf(data, LogLevel.Debug);
    
    /// <inheritdoc cref="BepInEx.Logging.ManualLogSource.LogInfo"/>
    public static void Info(params object?[] data) => LogSelf(data, LogLevel.Message);
    
    /// <inheritdoc cref="BepInEx.Logging.ManualLogSource.LogWarning"/>
    public static void Warning(params object?[] data) => LogSelf(data, LogLevel.Warning);
    
    /// <inheritdoc cref="BepInEx.Logging.ManualLogSource.LogError"/>
    public static void Error(params object?[] data) => LogSelf(data, LogLevel.Error);
}