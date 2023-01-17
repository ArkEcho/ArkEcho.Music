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
        public string UserName { get; set; } = string.Empty;

        [JsonProperty]
        public string Password { get; set; } = string.Empty;

        [JsonProperty]
        public bool DarkMode { get; set; } = true;

        [JsonProperty]
        public string MusicPathWindows { get; set; } = string.Empty;

        [JsonProperty]
        public Guid AccessToken { get; set; } = Guid.Empty;
    }

}
