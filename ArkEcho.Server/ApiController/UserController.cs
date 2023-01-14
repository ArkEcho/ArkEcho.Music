using ArkEcho.Core;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace ArkEcho.Server
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticateController : ArkEchoController
    {
        public AuthenticateController() : base("Authenticate")
        {
        }

        [HttpPost("Login")]
        public async Task<ActionResult> AuthenticateUserForLogin()
        {
            if (HttpContext.Request.ContentLength == 0)
            {
                Logger.LogImportant($"{Request.Path} Bad Request, Content is Empty!");
                return BadRequest();
            }

            string userRequestString = await getStringFromHttpBody();
            userRequestString = userRequestString.FromBase64();

            User user = new();
            await user.LoadFromJsonString(userRequestString);

            User checkedUser = await Server.AuthenticateUserForLoginAsync(user);

            return await checkUserMakeAnswer(checkedUser);
        }

        [HttpPost("Token")]
        public async Task<ActionResult> CheckUserToken()
        {
            if (HttpContext.Request.ContentLength == 0)
            {
                Logger.LogImportant($"{Request.Path} Bad Request, Content is Empty!");
                return BadRequest();
            }

            string guidString = await getStringFromHttpBody();

            Guid guid = new Guid(guidString);

            User checkedUser = await Server.CheckUserTokenAsync(guid);

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
    }
}
