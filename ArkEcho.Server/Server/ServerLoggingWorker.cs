using ArkEcho.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ArkEcho.Server
{
    public class ServerLoggingWorker : LoggingWorker
    {
        public class LogFile
        {
            public LogFile(string name, string fileName)
            {
                this.Name = name;
                this.FileName = fileName;
            }

            public string Name { get; set; } = string.Empty;
            public string FileName { get; set; } = string.Empty;

            public string GetFullFileName()
            {
                return $"{FileName}{logFileExtension}";
            }
        }

        private const string logFileExtension = ".log";
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
            LogFile file = logFiles.FindLast(x => x.Name == log.Name);

            if (file == null)
                file = createNewLogFile(log.Name, 0);
            else
            {
                FileInfo fileInfo = new FileInfo(file.GetFullFileName());

                if (fileInfo.Length > logFileSizeMax)
                {
                    List<LogFile> files = logFiles.FindAll(x => x.Name == file.Name);
                    if (files.Count >= maxFiles)
                    {
                        // Remove old File
                        LogFile fileToRemove = files[0];
                        File.Delete(fileToRemove.GetFullFileName());
                        files.Remove(fileToRemove);
                        logFiles.Remove(fileToRemove);

                        // Rotation -> Name_1 to Name_0; Name_2 to Name_1...
                        for (int i = 0; i < files.Count; i++)
                        {
                            LogFile fileToMod = files[i];
                            string oldFullFileName = fileToMod.GetFullFileName();
                            string oldFileName = fileToMod.FileName;

                            fileToMod.FileName = oldFileName.Substring(0, oldFileName.Length - 1) + i;

                            File.Move(oldFullFileName, fileToMod.GetFullFileName());
                        }
                    }

                    file = createNewLogFile(log.Name, files.Count);
                }
            }

            StreamWriter fs = null;

            try
            {
                fs = new StreamWriter(new FileStream(file.GetFullFileName(), FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite), Encoding.UTF8);
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

        private LogFile createNewLogFile(string name, int index)
        {
            string fileName = Path.Combine(logFolder, $"{DateTime.Now:yyyy-MM-dd_HH-mm-ss}_{name}_{index}");
            LogFile file = new LogFile(name, fileName);
            logFiles.Add(file);
            return file;
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
