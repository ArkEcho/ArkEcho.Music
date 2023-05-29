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
        public AuthenticateController(Server server) : base(server, "Authenticate")
        {
        }

        [HttpPost("{sessionToken}")]
        public async Task<ActionResult> GetUser(Guid sessionToken)
        {
            if (sessionToken == Guid.Empty)
                return BadRequest();

            User checkedUser = server.GetUserFromSessionToken(sessionToken);

            return await checkUserMakeAnswer(checkedUser);
        }

        [HttpPost("Login/{userName};{userPasswordEncrypted}")]
        public async Task<ActionResult> AuthenticateUserForLogin(string userName, string userPasswordEncrypted)
        {
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(userPasswordEncrypted))
                return BadRequest();

            User checkedUser = await server.AuthenticateUserForLoginAsync(userName, userPasswordEncrypted);

            return await checkUserMakeAnswer(checkedUser);
        }

        [HttpPost("Logout/{sessionToken}")]
        public async Task<ActionResult> LogoutUser(Guid sessionToken)
        {
            if (sessionToken == Guid.Empty)
                return BadRequest();

            server.LogoutSession(sessionToken);

            return Ok();
        }

        [HttpPost("SessionToken/{sessionToken}")]
        public async Task<ActionResult> CheckUserToken(Guid sessionToken)
        {
            if (sessionToken == Guid.Empty)
                return BadRequest();

            bool result = server.CheckSession(sessionToken);

            return result ? Ok() : NotFound();
        }

        [HttpPost("Update/{sessionToken}")]
        public async Task<ActionResult> UpdateUser(Guid sessionToken)
        {
            if (HttpContext.Request.ContentLength == 0 || sessionToken == Guid.Empty)
                return BadRequest();

            User user = await getUserFromHttpRequest();

            bool success = await server.UpdateUserAsync(user);

            return success ? Ok() : NotFound();
        }

        [HttpPost("ApiToken/{sessionToken}")]
        public async Task<ActionResult> GetApiToken(Guid sessionToken)
        {
            if (sessionToken == Guid.Empty)
                return BadRequest();

            bool result = server.CheckSession(sessionToken);

            return result ? Ok() : NotFound();
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
