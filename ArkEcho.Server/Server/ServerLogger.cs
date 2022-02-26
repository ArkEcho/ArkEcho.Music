using ArkEcho.Core;

namespace ArkEcho.Server
{
    public class ServerLogger : Logger
    {
        private const string logName = "Server";

        public ServerLogger(string context) : base(logName, context)
        {

        }

        protected override void transferLog(LogMessage log)
        {
            ArkEchoServer.Instance.AddLogMessage(log);
        }
    }
}
