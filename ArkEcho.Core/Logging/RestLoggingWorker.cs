using System.Threading.Tasks;

namespace ArkEcho.Core
{
    public class RestLoggingWorker : LoggingWorker
    {
        Rest restClient = null;

        public RestLoggingWorker(Rest restClient, Logging.LogLevel logLevel) : base(logLevel)
        {
            this.restClient = restClient;
        }

        protected override void HandleLogMessage(LogMessage log)
        {
            Task.Run(() => restClient.PostLogging(log)).Wait();
        }
    }
}
