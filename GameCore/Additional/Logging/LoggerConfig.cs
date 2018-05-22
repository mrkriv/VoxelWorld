using System.Collections.Generic;

namespace GameCore.Additional.Logging
{
    public class LoggerConfig
    {
        public IEnumerable<string> TargetTypes { get; set; } = new List<string>();
        public HashSet<string> ChanelBlackList { get; set; } = new HashSet<string>();
        public LoggerMessageType GeneralMessageType { get; set; }
    }
}