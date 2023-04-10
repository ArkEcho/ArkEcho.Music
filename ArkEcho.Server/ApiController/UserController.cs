using ArkEcho.Core;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace ArkEcho.Server
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticateController : BaseController
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

            User user = await getUserFromHttpRequest();

            User checkedUser = await Server.AuthenticateUserForLoginAsync(user);

            return await checkUserMakeAnswer(checkedUser);
        }

        [HttpPost("Logout")]
        public async Task<ActionResult> LogoutUser()
        {
            if (HttpContext.Request.ContentLength == 0)
            {
                Logger.LogImportant($"{Request.Path} Bad Request, Content is Empty!");
                return BadRequest();
            }

            string guidString = await getStringFromHttpBody();

            Guid guid = new Guid(guidString);

            Server.LogoutUser(guid);

            return Ok();
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

            User checkedUser = Server.CheckUserToken(guid);

            return await checkUserMakeAnswer(checkedUser);
        }

        [HttpPost("Update")]
        public async Task<ActionResult> UpdateUser()
        {
            if (HttpContext.Request.ContentLength == 0)
            {
                Logger.LogImportant($"{Request.Path} Bad Request, Content is Empty!");
                return BadRequest();
            }

            User user = await getUserFromHttpRequest();

            bool success = await Server.UpdateUserAsync(user);

            return success ? Ok() : BadRequest();
        }

        private async Task<User> getUserFromHttpRequest()
        {
            string userRequestString = await getStringFromHttpBody();
            userRequestString = userRequestString.FromBase64();

            User user = new User();
            await user.LoadFromJsonString(userRequestString);
            return user;
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
