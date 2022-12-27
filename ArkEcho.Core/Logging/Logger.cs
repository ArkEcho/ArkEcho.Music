using System;

namespace ArkEcho.Core
{
    public class Logger
    {
        public readonly string Name = string.Empty;

        public readonly string Context = string.Empty;

        private LoggingWorker worker = null;

        public Logger(string name, string context, LoggingWorker worker)
        {
            Name = name.ToUpper();
            Context = context.ToUpper();
            this.worker = worker;
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
                OriginGuid = worker.OriginGuid,
                Name = Name,
                Context = Context,
                Level = level,
                Message = message,
                TimeStamp = DateTime.Now
            };

            worker?.AddLogMessage(msg);
        }
    }
}
