using System.Collections.Concurrent;
using System.ComponentModel;
using System.Threading;

namespace ArkEcho.Core
{
    public abstract class LoggingWorker : BackgroundWorker
    {
        private bool stop = false;

        private ConcurrentQueue<LogMessage> loggingQueue = null;

        public LoggingWorker() : base()
        {
            loggingQueue = new ConcurrentQueue<LogMessage>();

            DoWork += LoggingWorker_DoWork;
        }

        public void AddLogMessage(LogMessage log)
        {
            loggingQueue.Enqueue(log);
        }

        private void LoggingWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            while (!stop)
            {
                if (loggingQueue.TryDequeue(out LogMessage log))
                {
                    HandleLogMessage(log);
                }
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