using System;

namespace ArkEcho.Core
{
    public class User : JsonBase
    {
        public const string UserTableName = "users";

        public enum UserTable
        {
            ID,
            USERNAME,
            PASSWORD
        }

        [JsonProperty]
        public int ID { get; set; }

        [JsonProperty]
        public string UserName { get; set; }

        [JsonProperty]
        public string Password { get; set; }

        [JsonProperty]
        public Guid AccessToken { get; set; } = Guid.Empty;
    }

}
