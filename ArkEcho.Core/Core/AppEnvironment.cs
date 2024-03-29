﻿using System;

namespace ArkEcho.Core
{
    public class AppEnvironment
    {
        public Guid AppGuid { get; set; } = Guid.NewGuid();

        public bool Development { get; set; } = false;

        public Resources.Platform Platform { get; set; } = Resources.Platform.None;

        public string AppName { get; set; } = string.Empty;

        // TODO: Better Name here and in Rest
        public bool UserHttpClientHandler { get; set; } = false;

        public string ServerAddress { get; set; } = string.Empty;

        public AppEnvironment(string appName, string serveraddress, bool development, Resources.Platform platform, bool userHttpClientHandler)
        {
            AppName = appName;

            ServerAddress = serveraddress;

            Development = development;
            Platform = platform;
            UserHttpClientHandler = userHttpClientHandler;
        }
    }
}
