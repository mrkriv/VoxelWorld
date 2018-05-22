using System;
using System.Collections.Generic;
using System.Linq;
using GameCore.Services;

namespace GameCore.Additional.Logging
{
    public class Logger
    {
        private List<ILoggerTarget> _targets { get; set; }
        private Config _config;

        public Logger(Config config, ServiceProvider serviceProvider)
        {
            _targets = new List<ILoggerTarget>();
            _config = config;

            foreach (var typeName in config.LoggerConfig.TargetTypes)
            {
                var type = Type.GetType(typeName);

                if (type == null)
                    throw new Exception($"Failed init logger: undefine type {typeName}");

                if (type.GetInterfaces().All(x => x != typeof(ILoggerTarget)))
                    throw new Exception($"Failed init logger: {type} is not implement ILoggerTarget");

                if (!serviceProvider.ContainsService(type))
                    serviceProvider.AddTransient(type);

                var service = serviceProvider.GetService(type);
                _targets.Add((ILoggerTarget) service);
            }
        }

        public void Log(LoggerMessageType type, string chanel, string message)
        {
            if(type < _config.LoggerConfig.GeneralMessageType)
                return;
            
            if(_config.LoggerConfig.ChanelBlackList.Contains(chanel))
                return;
            
            foreach (var target in _targets)
            {
                target.Print(chanel, type, message, DateTime.Now);
            }
        }

        public void Debug(string message, string chanel = "*")
        {
            Log(LoggerMessageType.Debug, chanel, message);
        }

        public void Log(string message, string chanel = "*")
        {
            Log(LoggerMessageType.Log, chanel, message);
        }

        public void Info(string message, string chanel = "*")
        {
            Log(LoggerMessageType.Info, chanel, message);
        }

        public void Warning(string message, string chanel = "*")
        {
            Log(LoggerMessageType.Warning, chanel, message);
        }

        public void Error(string message, string chanel = "*")
        {
            Log(LoggerMessageType.Error, chanel, message);
        }
    }
}