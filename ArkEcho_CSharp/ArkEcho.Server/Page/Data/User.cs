using System;

namespace ArkEcho.Server
{
    public class User
    {
        public string EmailAddress { get; set; }
        public string Password { get; set; }
        public Guid AccessToken { get; set; } = Guid.NewGuid();
    }

}
