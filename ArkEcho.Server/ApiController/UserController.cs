using ArkEcho.Core;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ArkEcho.Server
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        ArkEchoServer server = ArkEchoServer.Instance;

        [HttpPost("Login")]
        public async Task<ActionResult> AuthenticateUserForLogin()
        {
            if (HttpContext.Request.ContentLength == 0)
                return BadRequest();

            string userRequestString = await getStringFromHttpBody();
            userRequestString = userRequestString.FromBase64();

            User user = new User();
            await user.LoadFromJsonString(userRequestString);

            User checkedUser = server.AuthenticateUserForLogin(user);

            return await checkUserMakeAnswer(checkedUser);
        }

        [HttpPost("Token")]
        public async Task<ActionResult> CheckUserToken()
        {
            if (HttpContext.Request.ContentLength == 0)
                return BadRequest();

            string guidString = await getStringFromHttpBody();

            Guid guid = new Guid(guidString);

            User checkedUser = server.CheckUserToken(guid);

            return await checkUserMakeAnswer(checkedUser);
        }

        private async Task<ActionResult> checkUserMakeAnswer(User user)
        {
            if (user != null)
            {
                string userResultString = await user.SaveToJsonString();
                return Ok(userResultString.ToBase64());
            }
            else
                return BadRequest();
        }

        private async Task<string> getStringFromHttpBody()
        {
            Stream req = HttpContext.Request.Body;

            return await new StreamReader(req).ReadToEndAsync();
        }
    }
}
