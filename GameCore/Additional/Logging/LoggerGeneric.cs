namespace GameCore.Additional.Logging
{
    public class Logger<TChanerOwner> where TChanerOwner : class
    {
        private readonly string _chanelName;
        private readonly Logger _logger;

        public Logger(Logger logger)
        {
            _chanelName = typeof(TChanerOwner).Name;
            _logger = logger;
        }
        
        public void Debug(string message)
        {
            _logger.Log(LoggerMessageType.Debug, _chanelName, message);
        }

        public void Log(string message, string chanel = "*")
        {
            _logger.Log(LoggerMessageType.Log, _chanelName, message);
        }

        public void Info(string message, string chanel = "*")
        {
            _logger.Log(LoggerMessageType.Info, _chanelName, message);
        }

        public void Warning(string message, string chanel = "*")
        {
            _logger.Log(LoggerMessageType.Warning, _chanelName, message);
        }

        public void Error(string message, string chanel = "*")
        {
            _logger.Log(LoggerMessageType.Error, _chanelName, message);
        }
    }
}