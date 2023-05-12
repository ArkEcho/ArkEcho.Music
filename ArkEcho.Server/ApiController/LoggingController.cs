using ArkEcho.Core;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ArkEcho.Server
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoggingController : BaseController
    {
        public LoggingController(Server server) : base(server, "Logging")
        {
        }

        [HttpPost]
        public async Task<ActionResult> LoggingPost()
        {
            if (HttpContext.Request.ContentLength == 0)
                return BadRequest();

            string requestString = await getStringFromHttpBody();
            requestString = requestString.FromBase64();

            LogMessage message = new();
            await message.LoadFromJsonString(requestString);

            server.LoggingWorker.AddLogMessage(message);

            return Ok();
        }
    }
}
