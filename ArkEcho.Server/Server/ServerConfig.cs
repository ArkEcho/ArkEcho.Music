﻿using ArkEcho.Core;
using System;

namespace ArkEcho.Server
{
    public class ServerConfig : JsonBase
    {
        public ServerConfig(string FileName) : base(FileName) { }

        [JsonProperty]
        public Uri LoggingFolder { get; private set; } = new Uri("about:blank");

        [JsonProperty]
        public Uri DatabasePath { get; private set; } = new Uri("about:blank");

        [JsonProperty]
        public Logging.LogLevel LogLevel { get; set; } = Logging.LogLevel.Important;
    }
}
