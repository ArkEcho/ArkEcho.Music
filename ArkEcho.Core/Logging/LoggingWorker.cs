using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace ArkEcho.Core
{
    public abstract class LoggingWorker : IDisposable
    {
        private bool stop = false;

        private ConcurrentQueue<LogMessage> loggingQueue = null;

        private Logging.LogLevel logLevel;

        public Guid OriginGuid { get; private set; }

        public LoggingWorker(Logging.LogLevel logLevel) : base()
        {
            this.logLevel = logLevel;
            loggingQueue = new ConcurrentQueue<LogMessage>();
            OriginGuid = Guid.NewGuid();
        }

        public void AddLogMessage(LogMessage log)
        {
            loggingQueue.Enqueue(log);
        }

        public async Task Start()
        {
            while (!stop)
            {
                if (loggingQueue.Count == 0)
                {
                    await Task.Delay(500);
                    continue;
                }

                if (loggingQueue.TryDequeue(out LogMessage log) && log.Level <= logLevel)
                    HandleLogMessage(log);
            }
        }

        protected abstract void HandleLogMessage(LogMessage log);

        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    stop = true;
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
        }
    }
}