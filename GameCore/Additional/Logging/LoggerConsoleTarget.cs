using System;
using System.Threading.Tasks;

namespace GameCore.Additional.Logging
{
    public class LoggerConsoleTarget : ILoggerTarget
    {
        private static readonly object consoleLoc = new object();

        public Task Print(string chanel, LoggerMessageType type, string message, DateTime time)
        {
            lock (consoleLoc)
            {
                Console.ForegroundColor = GetColor(type);
                Console.WriteLine($"[{time:dd.MM.yy HH:mm:ss.fff}] [{chanel}] {message}");
            }

            return Task.CompletedTask;
        }

        private ConsoleColor GetColor(LoggerMessageType messageType)
        {
            switch (messageType)
            {
                case LoggerMessageType.Debug:
                    return ConsoleColor.Cyan;
                case LoggerMessageType.Log:
                    return ConsoleColor.DarkGray;
                case LoggerMessageType.Info:
                    return ConsoleColor.Gray;
                case LoggerMessageType.Warning:
                    return ConsoleColor.DarkYellow;
                case LoggerMessageType.Error:
                    return ConsoleColor.DarkRed;
                default:
                    throw new ArgumentOutOfRangeException(nameof(messageType), messageType, null);
            }
        }
    }
}