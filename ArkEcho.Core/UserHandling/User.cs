using System;

namespace ArkEcho.Core
{
    public class User : JsonBase
    {
        public enum UserTable
        {
            USERNAME,
            PASSWORD
        }

        [JsonProperty]
        public string UserName { get; set; }

        [JsonProperty]
        public string Password { get; set; }

        [JsonProperty]
        public Guid AccessToken { get; set; } = Guid.Empty;
    }

}
