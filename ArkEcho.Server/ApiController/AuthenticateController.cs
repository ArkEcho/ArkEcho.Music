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

        [HttpPost()]
        public async Task<ActionResult> GetUser([FromQuery] Guid sessionToken)
        {
            if (sessionToken == Guid.Empty)
                return BadRequest();

            User checkedUser = server.GetUserFromSessionToken(sessionToken);

            return await checkUserMakeAnswer(checkedUser);
        }

        [HttpPost("Login")]
        public async Task<ActionResult> AuthenticateUserForLogin([FromQuery] string userName, [FromQuery] string userPassword)
        {
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(userPassword))
                return BadRequest();

            User checkedUser = await server.AuthenticateUserForLoginAsync(userName, userPassword);

            return await checkUserMakeAnswer(checkedUser);
        }

        [HttpPost("Logout")]
        public async Task<ActionResult> LogoutUser([FromQuery] Guid sessionToken)
        {
            if (sessionToken == Guid.Empty)
                return BadRequest();

            server.LogoutSession(sessionToken);

            return server.LogoutSession(sessionToken) ? Ok() : NotFound();
        }

        [HttpPost("SessionToken")]
        public async Task<ActionResult> CheckUserToken([FromQuery] Guid sessionToken)
        {
            if (sessionToken == Guid.Empty)
                return BadRequest();

            bool result = server.CheckSession(sessionToken);

            return result ? Ok() : NotFound();
        }

        [HttpPost("Update")]
        public async Task<ActionResult> UpdateUser([FromQuery] Guid sessionToken)
        {
            if (HttpContext.Request.ContentLength == 0 || sessionToken == Guid.Empty)
                return BadRequest();

            User user = await getUserFromHttpRequest();

            bool success = await server.UpdateUserAsync(user);

            return success ? Ok() : NotFound();
        }

        [HttpPost("ApiToken")]
        public async Task<ActionResult> GetApiToken([FromQuery] Guid sessionToken)
        {
            if (sessionToken == Guid.Empty)
                return BadRequest();

            Guid apiToken = server.GetApiToken(sessionToken);
            return apiToken != Guid.Empty ? Ok(apiToken.ToString()) : NotFound();
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
