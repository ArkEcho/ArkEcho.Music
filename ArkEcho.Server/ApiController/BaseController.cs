using ArkEcho.Core;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ArkEcho.Server
{
    public abstract class BaseController : ControllerBase
    {
        protected Logger Logger { get; private set; } = null;

        protected string ControllerName { get; private set; } = string.Empty;

        protected Server server = null;

        public BaseController(Server server, string controllerName) : base()
        {
            this.server = server;
            this.ControllerName = controllerName;
            Logger = new FileLogger(server.Environment, $"{controllerName}-REST", server.LoggingWorker);
        }

        protected async Task<string> getStringFromHttpBody()
        {
            Stream req = HttpContext.Request.Body;
            return await new StreamReader(req).ReadToEndAsync();
        }

        protected bool checkApiToken(Guid apiToken)
        {
            return server.CheckApiToken(apiToken);
        }

        protected bool checkApiToken()
        {
            string apiToken = HttpContext.Request.Headers[Resources.ApiTokenHeaderKey];
            if (!Guid.TryParse(apiToken, out Guid guid))
                return false;
            return checkApiToken(guid);
        }
    }
}
