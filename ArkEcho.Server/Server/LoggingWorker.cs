using ArkEcho.Core;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading;

namespace ArkEcho.Server
{
    public class LoggingWorker : BackgroundWorker
    {
        private bool stop = false;
        private bool working = false;
        private string logFolder = string.Empty;

        private ConcurrentQueue<LogMessage> loggingQueue = null;

        private Dictionary<string, StreamWriter> logFiles = null;

        public LoggingWorker(string logFolder) : base()
        {
            this.logFolder = logFolder;
            loggingQueue = new();
            logFiles = new();

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
                    writeLogMessage(log);
            }

            working = false;
        }

        private void writeLogMessage(LogMessage log)
        {
            StreamWriter fs = getFileStream(log.Origin);

            fs?.WriteLine(log.ToLogString());
            fs?.Flush();
        }

        private StreamWriter getFileStream(Logger logger)
        {
            // TODO: Log File Rotation, open new if Filestream voll

            StreamWriter fs = null;
            if (!logFiles.TryGetValue(logger.Name, out fs))
            {
                try
                {
                    fs = new StreamWriter(getLogFileName(logger), true, System.Text.Encoding.UTF8);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception opening Logging FileStream: {ex.Message}");
                }

                logFiles.Add(logger.Name, fs);
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

                    for (int i = 0; i < 10; i++)
                        Thread.Sleep(500);

                    foreach (StreamWriter fs in logFiles.Values)
                    {
                        fs.Flush();
                        fs.Dispose();
                    }
                }
            }

            base.Dispose(Disposing);
        }
    }
}