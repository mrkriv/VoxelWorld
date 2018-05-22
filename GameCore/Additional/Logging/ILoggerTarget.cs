using System;
using System.Threading.Tasks;

namespace GameCore.Additional.Logging
{
    public interface ILoggerTarget
    {
        Task Print(string chanel, LoggerMessageType type, string message, DateTime time);
    }
}