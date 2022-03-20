using System.Threading.Tasks;

namespace ArkEcho.Core
{
    public class RestLoggingWorker : LoggingWorker
    {
        ArkEchoRest restClient = null;

        public RestLoggingWorker(ArkEchoRest restClient, Logging.LogLevel logLevel) : base(logLevel)
        {
            this.restClient = restClient;
        }

        protected override void HandleLogMessage(LogMessage log)
        {
            Task.Run(() => restClient.PostLogging(log)).Wait();
        }
    }
}
