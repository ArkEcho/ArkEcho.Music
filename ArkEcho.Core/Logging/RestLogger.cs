using System.Threading.Tasks;

namespace ArkEcho.Core
{
    public class RestLogger : Logger
    {
        private Rest rest = null;
        public RestLogger(AppEnvironment environment, string context, Rest rest) : base(environment, context)
        {
            this.rest = rest;
        }

        protected override async Task<bool> logMessage(LogMessage msg)
        {
            return await rest.PostLogging(msg);
        }
    }
}
