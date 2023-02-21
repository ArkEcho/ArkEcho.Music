using ArkEcho.Core;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;

namespace ArkEcho.Server
{
    public abstract class BaseController : ControllerBase
    {
        protected Logger Logger { get; private set; } = null;

        protected string ControllerName { get; private set; } = string.Empty;

        protected Server Server { get; private set; } = Server.Instance;

        public BaseController(string controllerName) : base()
        {
            this.ControllerName = controllerName;
            Logger = new FileLogger(Server.Instance.Environment, $"{controllerName}-REST", Server.Instance.LoggingWorker);
        }

        protected async Task<string> getStringFromHttpBody()
        {
            Stream req = HttpContext.Request.Body;
            return await new StreamReader(req).ReadToEndAsync();
        }
    }
}
