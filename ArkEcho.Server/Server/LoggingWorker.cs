using ArkEcho.Core;
using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Threading;

namespace ArkEcho.Server
{
    public class LoggingWorker : BackgroundWorker
    {
        private bool stop = false;
        private bool working = false;
        private string logFolder = string.Empty;

        private ConcurrentQueue<LogMessage> loggingQueue = null;

        public LoggingWorker(string logFolder) : base()
        {
            this.logFolder = logFolder;
            loggingQueue = new();

            DoWork += LoggingWorker_DoWork;
        }

        public void AddLogMessage(LogMessage log)
        {
            loggingQueue.Enqueue(log);
        }

        private void LoggingWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            working = true;

            while (!stop)
            {
                if (loggingQueue.TryDequeue(out LogMessage log))
                {
                    using (StreamWriter fs = getFileStream(log.Origin))
                    {
                        fs?.WriteLine(log.ToLogString());
                        fs?.Flush();
                    }
                }
            }

            working = false;
        }

        private StreamWriter getFileStream(Logger logger)
        {
            // TODO: Log File Rotation, open new if Filestream voll

            StreamWriter fs = null;
            string logFileName = getLogFileName(logger);

            try
            {
                fs = new StreamWriter(new FileStream(getLogFileName(logger), FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite), Encoding.UTF8);
                fs.BaseStream.Seek(0, SeekOrigin.End);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception opening Logging FileStream: {ex.Message}");
            }

            return fs;
        }

        private string getLogFileName(Logger logger)
        {
            return Path.Combine(logFolder, $"{DateTime.Now:yyyy-MM-dd_HH-mm-ss}_{logger.Name}.log");
        }

        bool disposed = false;

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