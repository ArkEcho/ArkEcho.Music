using ArkEcho.Core;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ArkEcho.Server
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        ArkEchoServer server = ArkEchoServer.Instance;

        [HttpGet("Authenticate")]
        public async Task<ActionResult> AuthenticateUser()
        {
            User user = await getUserFromHttpBody();

            User checkedUser = server.CheckUserForLogin(user);
            return Ok(await checkedUser.SaveToJsonString());
        }

        [HttpGet("Authenticate/{guid}")]
        public async Task<ActionResult> CheckUserToken(Guid guid)
        {
            User checkedUser = server.CheckUserToken(guid);
            return Ok(await checkedUser.SaveToJsonString());
        }

        private async Task<User> getUserFromHttpBody()
        {
            Stream req = HttpContext.Request.Body;
            req.Seek(0, System.IO.SeekOrigin.Begin);
            string json = new StreamReader(req).ReadToEnd();

            User user = new User();
            await user.LoadFromJsonString(json);
            return user;
        }
    }
}
