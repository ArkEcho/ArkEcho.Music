using ArkEcho.Core;
using System;
using System.IO;
using System.Text;

namespace ArkEcho.Server
{
    public class ServerLoggingWorker : LoggingWorker
    {
        private string logFolder = string.Empty;

        public ServerLoggingWorker(string logFolder, Logging.LogLevel logLevel) : base(logLevel)
        {
            this.logFolder = logFolder;
        }

        protected override void HandleLogMessage(LogMessage log)
        {
            using (StreamWriter fs = getFileStream(log.Origin))
            {
                fs?.WriteLine(log.ToLogString());
                fs?.Flush();
            }
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
