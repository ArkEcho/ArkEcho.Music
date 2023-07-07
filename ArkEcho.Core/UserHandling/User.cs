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
            MUSICLIBRARYPATH,
            SETTINGS
        }

        [JsonProperty]
        public int ID { get; set; } = 0;

        [JsonProperty]
        public string UserName { get; set; } = string.Empty;

        [JsonProperty]
        public string Password { get; set; } = string.Empty;

        [JsonProperty]
        public Uri MusicLibraryPath { get; set; }

        [JsonProperty]
        public Guid SessionToken { get; set; } = Guid.Empty;

        [JsonProperty]
        public UserSettings Settings { get; set; } = new UserSettings();
    }

}
