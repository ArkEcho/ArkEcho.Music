using System;
using System.Collections.Generic;

namespace ArkEcho.Core
{
    public class UserSettings : JsonBase
    {
        [JsonProperty]
        public bool DarkMode { get; set; } = true;

        [JsonProperty]
        public List<UserPath> MusicPathList { get; set; } = new List<UserPath>();

        public class UserPath : JsonBase
        {
            [JsonProperty]
            public string MachineName { get; set; } = string.Empty;

            [JsonProperty]
            public Uri Path { get; set; } = new Uri("about:blank");
        }

        public UserPath GetLocalUserSettings()
        {
            return MusicPathList.Find(x => x.MachineName.Equals(System.Environment.MachineName, StringComparison.OrdinalIgnoreCase));
        }
    }
}
