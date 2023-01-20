using System;
using System.Threading.Tasks;

namespace ArkEcho.Core
{
    public abstract class Logger
    {
        protected AppEnvironment environment = null;

        protected string context = string.Empty;

        public Logger(AppEnvironment environment, string context)
        {
            this.environment = environment;
            this.context = context.ToUpper();
        }

        public void LogStatic(string message)
        {
            Log(message, Logging.LogLevel.Static);
        }

        public void LogError(string message)
        {
            Log(message, Logging.LogLevel.Error);
        }

        public void LogImportant(string message)
        {
            Log(message, Logging.LogLevel.Important);
        }

        public void LogDebug(string message)
        {
            Log(message, Logging.LogLevel.Debug);
        }

        public void Log(string message, Logging.LogLevel level)
        {
            LogMessage msg = new LogMessage()
            {
                OriginGuid = environment.AppGuid,
                Name = environment.AppName,
                Context = context,
                Level = level,
                Message = message,
                TimeStamp = DateTime.Now
            };

            logMessage(msg);
        }

        protected abstract Task<bool> logMessage(LogMessage msg);
    }
}
