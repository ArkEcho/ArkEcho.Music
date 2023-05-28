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
            PASSWORD,
            SETTINGS
        }

        [JsonProperty]
        public int ID { get; set; }

        [JsonProperty]
        public string UserName { get; set; } = string.Empty;

        [JsonProperty]
        public string Password { get; set; } = string.Empty;

        [JsonProperty]
        public Guid SessionToken { get; set; } = Guid.Empty;

        [JsonProperty]
        public UserSettings Settings { get; set; } = new UserSettings();
    }

}
