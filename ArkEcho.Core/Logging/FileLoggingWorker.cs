using ArkEcho.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ArkEcho.Server
{
    public class FileLoggingWorker : LoggingWorker
    {
        private class LogFile
        {
            public LogFile(LogMessage log, int index)
            {
                this.OriginGuid = log.OriginGuid;
                this.Name = log.Name;
                this.Index = index;
            }

            public Guid OriginGuid { get; set; } = Guid.Empty;
            public string Name { get; set; } = string.Empty;
            public int Index { get; set; } = 0;
            public DateTime CreationTime { get; set; } = DateTime.Now;

            public string GetFullFileName(string logFolder)
            {
                return Path.Combine(logFolder, $"{CreationTime:yyyy-MM-dd_HH-mm-ss}_{Name}_{Index}") + logFileExtension;
            }
        }

        private const string logFileExtension = ".log";
        private string logFolder = string.Empty;

        private int maxFiles = 10;
        private long logFileSizeMax = 10485760; // 10mb
        //private long logFileSizeMax = 548570; // 500kb

        private List<LogFile> logFiles = new List<LogFile>();

        public FileLoggingWorker(string logFolder, Logging.LogLevel logLevel) : base(logLevel)
        {
            this.logFolder = logFolder;
        }

        protected override void HandleLogMessage(LogMessage log)
        {
            LogFile file = getLogFile(log);

            try
            {
                if (!Directory.Exists(logFolder))
                    Directory.CreateDirectory(logFolder);

                using (StreamWriter fs = new StreamWriter(new FileStream(file.GetFullFileName(logFolder), FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read), Encoding.UTF8))
                {
                    fs.BaseStream.Seek(0, SeekOrigin.End);
                    fs.WriteLine(log.ToLogString());
                    fs.Flush();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception writing Log: {ex.GetFullMessage()}");
            }
        }

        private LogFile getLogFile(LogMessage log)
        {
            LogFile file = null;
            List<LogFile> files = logFiles.FindAll(x => x.Name == log.Name && x.OriginGuid == log.OriginGuid);

            if (files.IsNullOrEmpty()) // First log File
                file = createNewLogFile(log, 0);
            else
            {
                file = files[files.Count - 1];
                FileInfo fileInfo = new FileInfo(file.GetFullFileName(logFolder));

                if (!fileInfo.Exists)
                {
                    int oldIndex = file.Index;
                    checkAndDeleteFilesFromLists(files);

                    file = createNewLogFile(log, oldIndex);
                }
                else if (fileInfo.Length > logFileSizeMax) // Create new if its too big
                {
                    if (file.Index == maxFiles - 1)
                        deleteFirstAndRotateFiles(files);

                    file = createNewLogFile(log, files[files.Count - 1].Index + 1);
                }
            }

            return file;
        }

        private void checkAndDeleteFilesFromLists(List<LogFile> files)
        {
            List<LogFile> copyFiles = new List<LogFile>(files);
            foreach (LogFile logFile in copyFiles)
            {
                if (!File.Exists(logFile.GetFullFileName(logFolder)))
                {
                    files.Remove(logFile);
                    logFiles.Remove(logFile);
                }
            }
        }

        private void deleteFirstAndRotateFiles(List<LogFile> files)
        {
            if (files.IsNullOrEmpty())
                return;

            checkAndDeleteFilesFromLists(files);

            // Remove oldest File
            if (files[0].Index == 0)
            {
                LogFile fileToRemove = files[0];
                File.Delete(fileToRemove.GetFullFileName(logFolder));
                files.Remove(fileToRemove);
                logFiles.Remove(fileToRemove);
            }

            // Rotation -> Name_1 to Name_0; Name_2 to Name_1...
            foreach (LogFile file in files)
            {
                LogFile fileToRotate = file;
                string oldFullFileName = fileToRotate.GetFullFileName(logFolder);
                fileToRotate.Index--;

                File.Move(oldFullFileName, fileToRotate.GetFullFileName(logFolder));
            }
        }

        private LogFile createNewLogFile(LogMessage log, int index)
        {
            LogFile file = new LogFile(log, index);
            logFiles.Add(file);

            // TODO: Guid und Datei Header schreiben?

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
