using ArkEcho.Core;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;

namespace ArkEcho.Server
{
    public abstract class ArkEchoController : ControllerBase
    {
        protected Logger Logger { get; private set; } = null;

        protected string ControllerName { get; private set; } = string.Empty;

        protected ArkEchoServer Server { get; private set; } = ArkEchoServer.Instance;

        public ArkEchoController(string controllerName) : base()
        {
            this.ControllerName = controllerName;
            Logger = new Logger("Server", $"{controllerName}-REST", ArkEchoServer.Instance.LoggingWorker);
        }

        protected async Task<string> getStringFromHttpBody()
        {
            Stream req = HttpContext.Request.Body;
            return await new StreamReader(req).ReadToEndAsync();
        }
    }
}
