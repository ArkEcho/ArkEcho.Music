using ArkEcho.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ArkEcho.Server
{
    public class ServerLoggingWorker : LoggingWorker
    {
        private struct LogFile
        {
            public LogFile(string name, string fileName)
            {
                this.Name = name;
                this.FileName = fileName;
            }

            public string Name { get; private set; }
            public string FileName { get; private set; }
        }

        private string logFileExtension = ".log";
        private string logFolder = string.Empty;
        private int maxFiles = 10;
        private long logFileSizeMax = 10485760; // 10mb

        private List<LogFile> logFiles = new List<LogFile>();

        public ServerLoggingWorker(string logFolder, Logging.LogLevel logLevel) : base(logLevel)
        {
            this.logFolder = logFolder;
        }

        protected override void HandleLogMessage(LogMessage log)
        {
            StreamWriter fs = null;

            LogFile file = logFiles.Find(x => x.Name == log.Name);
            if (string.IsNullOrEmpty(file.FileName))
            {
                file = new LogFile(log.Name, getNewLogFileName(log)); // New Logging/File
                logFiles.Add(file);
            }
            else
            {
                //FileInfo fileInfo = new FileInfo(fileName);
                //if (fileInfo.Length > logFileSizeMax)
                //{
                //    List<string> files = logFiles.V;
                //}
            }

            try
            {
                fs = new StreamWriter(new FileStream($"{file.FileName}{logFileExtension}", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite), Encoding.UTF8);
                fs.BaseStream.Seek(0, SeekOrigin.End);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception opening Logging FileStream: {ex.Message}");
            }

            using (fs)
            {
                fs?.WriteLine(log.ToLogString());
                fs?.Flush();
            }
        }

        private string getNewLogFileName(LogMessage logger)
        {
            return Path.Combine(logFolder, $"{DateTime.Now:yyyy-MM-dd_HH-mm-ss}_{logger.Name}_0");
        }

        private bool disposed = false;

        protected override void Dispose(bool Disposing)
        {
            if (!disposed)
            {
                if (Disposing)
                {
                }
            }

            base.Dispose(Disposing);
        }
    }
}
