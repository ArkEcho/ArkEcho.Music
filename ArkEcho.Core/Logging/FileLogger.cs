using ArkEcho.Server;
using System.Threading.Tasks;

namespace ArkEcho.Core
{
    public class FileLogger : Logger
    {
        private FileLoggingWorker worker = null;
        public FileLogger(AppEnvironment environment, string context, FileLoggingWorker worker) : base(environment, context)
        {
            this.worker = worker;
        }

        protected override async Task<bool> logMessage(LogMessage msg)
        {
            worker.AddLogMessage(msg);
            return true;
        }
    }
}
