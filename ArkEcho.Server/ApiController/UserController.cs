using ArkEcho.Core;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
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

        [HttpPost]
        public async Task<ActionResult> GetUser()
        {
            if (HttpContext.Request.ContentLength == 0)
            {
                Logger.LogImportant($"{Request.Path} Bad Request, Content is Empty!");
                return BadRequest();
            }

            User checkedUser = server.GetUserFromSessionToken(await getGuidFromHttpRequest());

            return await checkUserMakeAnswer(checkedUser);
        }

        [HttpPost("Login")]
        public async Task<ActionResult> AuthenticateUserForLogin()
        {
            if (HttpContext.Request.ContentLength == 0)
            {
                Logger.LogImportant($"{Request.Path} Bad Request, Content is Empty!");
                return BadRequest();
            }

            List<string> content = (await getStringFromHttpBody()).FromBase64().Split(';', StringSplitOptions.RemoveEmptyEntries).ToList();
            if (content.Count != 2)
                return BadRequest();

            User checkedUser = await server.AuthenticateUserForLoginAsync(content[0], content[1]);

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

            server.LogoutSession(await getGuidFromHttpRequest());

            return Ok();
        }

        [HttpPost("SessionToken")]
        public async Task<ActionResult> CheckUserToken()
        {
            if (HttpContext.Request.ContentLength == 0)
            {
                Logger.LogImportant($"{Request.Path} Bad Request, Content is Empty!");
                return BadRequest();
            }

            bool result = server.CheckSession(await getGuidFromHttpRequest());

            return result ? Ok() : NotFound();
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

            bool success = await server.UpdateUserAsync(user);

            return success ? Ok() : NotFound();
        }

        [HttpPost("ApiToken")]
        public async Task<ActionResult> GetApiToken()
        {
            if (HttpContext.Request.ContentLength == 0)
            {
                Logger.LogImportant($"{Request.Path} Bad Request, Content is Empty!");
                return BadRequest();
            }

            bool result = server.CheckSession(await getGuidFromHttpRequest());

            return result ? Ok() : NotFound();
        }

        private async Task<Guid> getGuidFromHttpRequest()
        {
            string guidString = await getStringFromHttpBody();
            if (Guid.TryParse(guidString, out Guid guid))
                return guid;
            else
                return Guid.Empty;
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
