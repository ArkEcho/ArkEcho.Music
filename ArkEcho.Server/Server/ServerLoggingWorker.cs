using ArkEcho.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ArkEcho.Server
{
    public class ServerLoggingWorker : LoggingWorker
    {
        private class LogFile
        {
            public LogFile(Guid guid, string name, string fileName)
            {
                this.OriginGuid = guid;
                this.Name = name;
                this.FileName = fileName;
            }

            public Guid OriginGuid { get; set; } = Guid.Empty;
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
            // TODO: Error als S\ geloggt von App kommend -> Enums in JsonBase

            LogFile file = null;
            List<LogFile> files = logFiles.FindAll(x => x.Name == log.Name && x.OriginGuid == log.OriginGuid);

            if (files.Count == 0) // First log File
                file = createNewLogFile(log.OriginGuid, log.Name, 0);
            else
            {
                file = files[files.Count - 1];
                FileInfo fileInfo = new FileInfo(file.GetFullFileName());

                // TODO: Wenn Datei(en) gelöscht Exception
                if (fileInfo.Length > logFileSizeMax) // Create new if its too big
                {
                    if (files.Count >= maxFiles)
                        deleteAndRotateFiles(files);

                    file = createNewLogFile(log.OriginGuid, log.Name, files.Count);
                }
            }

            try
            {
                if (!Directory.Exists(logFolder))
                    Directory.CreateDirectory(logFolder);

                using (StreamWriter fs = new StreamWriter(new FileStream(file.GetFullFileName(), FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite), Encoding.UTF8))
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

        private void deleteAndRotateFiles(List<LogFile> files)
        {
            // Remove old File
            LogFile fileToRemove = files[0];
            File.Delete(fileToRemove.GetFullFileName());
            files.Remove(fileToRemove);
            logFiles.Remove(fileToRemove);

            // Rotation -> Name_1 to Name_0; Name_2 to Name_1...
            for (int i = 0; i < files.Count; i++)
            {
                LogFile fileToRotate = files[i];
                string oldFullFileName = fileToRotate.GetFullFileName();
                string oldFileName = fileToRotate.FileName;

                fileToRotate.FileName = oldFileName.Substring(0, oldFileName.Length - 1) + i;

                File.Move(oldFullFileName, fileToRotate.GetFullFileName());
            }
        }

        private LogFile createNewLogFile(Guid originGuid, string name, int index)
        {
            string fileName = Path.Combine(logFolder, $"{DateTime.Now:yyyy-MM-dd_HH-mm-ss}_{name}_{index}");

            LogFile file = new LogFile(originGuid, name, fileName);
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
