using System;

namespace ArkEcho.Core
{
    public abstract class Logger : JsonBase
    {
        [JsonProperty]
        public string Name { get; set; }

        [JsonProperty]
        public string Context { get; set; }

        public Logger(string name, string context)
        {
            Name = name.ToUpper();
            Context = context.ToUpper();
        }

        public void LogStatic(string message)
        {
            log(message, Logging.LogLevel.Static);
        }

        public void LogError(string message)
        {
            log(message, Logging.LogLevel.Error);
        }

        public void LogImportant(string message)
        {
            log(message, Logging.LogLevel.Important);
        }

        public void LogDebug(string message)
        {
            log(message, Logging.LogLevel.Debug);
        }

        private void log(string message, Logging.LogLevel level)
        {
            LogMessage msg = new LogMessage()
            {
                Level = level,
                Message = message,
                Origin = this,
                TimeStamp = DateTime.Now
            };

            transferLog(msg);
        }

        protected abstract void transferLog(LogMessage log);
    }
}
