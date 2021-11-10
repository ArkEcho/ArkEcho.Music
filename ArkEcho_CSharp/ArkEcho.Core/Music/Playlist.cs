using System;
using System.Collections.Generic;

namespace ArkEcho.Core
{
    public class Playlist : JsonBase
    {
        [JsonProperty]
        public string Name { get; private set; } = string.Empty;

        [JsonProperty]
        public Guid GUID { get; private set; } = new Guid();

        [JsonProperty]
        public SortedSet<Guid> MusicFiles { get; private set; } = new SortedSet<Guid>();

        public Playlist(string Name) : base()
        {
            this.Name = Name;
        }
    }
}
