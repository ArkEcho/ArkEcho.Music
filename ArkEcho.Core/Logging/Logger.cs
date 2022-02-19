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

        public void Log(string message, LogLevel level)
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
