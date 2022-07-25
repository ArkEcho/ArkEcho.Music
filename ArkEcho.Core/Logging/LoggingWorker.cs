using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Threading;

namespace ArkEcho.Core
{
    public abstract class LoggingWorker : BackgroundWorker
    {
        private bool stop = false;

        private ConcurrentQueue<LogMessage> loggingQueue = null;

        private Logging.LogLevel logLevel;

        public Guid OriginGuid { get; private set; }

        public LoggingWorker(Logging.LogLevel logLevel) : base()
        {
            this.logLevel = logLevel;
            loggingQueue = new ConcurrentQueue<LogMessage>();
            DoWork += LoggingWorker_DoWork;
            OriginGuid = Guid.NewGuid();
        }

        public void AddLogMessage(LogMessage log)
        {
            loggingQueue.Enqueue(log);
        }

        private void LoggingWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            while (!stop)
            {
                if (loggingQueue.TryDequeue(out LogMessage log) && log.Level <= logLevel)
                    HandleLogMessage(log);
            }
        }

        protected abstract void HandleLogMessage(LogMessage log);

        private bool disposed = false;

        protected override void Dispose(bool Disposing)
        {
            if (!disposed)
            {
                if (Disposing)
                {
                    stop = true;

                    Thread.Sleep(1000);
                }
            }

            base.Dispose(Disposing);
        }
    }
}