using System.Threading.Tasks;

namespace ArkEcho.Core
{
    public class RestLoggingWorker : LoggingWorker
    {
        IRestLogging restClient = null;

        public RestLoggingWorker(IRestLogging restClient, Logging.LogLevel logLevel) : base(logLevel)
        {
            this.restClient = restClient;
        }

        protected override void HandleLogMessage(LogMessage log)
        {
            Task.Run(() => restClient.PostLogging(log)).Wait();
        }
    }
}
