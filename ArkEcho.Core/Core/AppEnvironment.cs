using System;

namespace ArkEcho.Core
{
    public class AppEnvironment
    {
        public Guid AppGuid { get; set; } = Guid.NewGuid();

        public bool Development { get; set; } = false;

        public Resources.Platform Platform { get; set; } = Resources.Platform.None;

        public string AppName { get; set; } = string.Empty;

        public string MusicPathAndroid { get; set; } = string.Empty;

        // TODO: Better Name here and in Rest
        public bool UserHttpClientHandler { get; set; } = false;

        public AppEnvironment(string appName, bool development, Resources.Platform platform, bool userHttpClientHandler)
        {
            AppName = appName;
            Development = development;
            Platform = platform;
            UserHttpClientHandler = userHttpClientHandler;
        }
    }
}
