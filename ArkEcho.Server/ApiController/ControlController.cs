using Microsoft.AspNetCore.Mvc;

namespace ArkEcho.Server
{
    [Route("api/[controller]")]
    [ApiController]
    public class ControlController : BaseController
    {
        public ControlController(Server server) : base(server, "Control")
        {
        }

        [HttpGet()]
        [HttpPost()]
        [Route("")]
        public ActionResult Standard()
        {
            return Ok();
        }
    }
}
