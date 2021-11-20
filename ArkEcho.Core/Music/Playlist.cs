using System;
using System.Collections.Generic;

namespace ArkEcho.Core
{
    public class Playlist : JsonBase
    {
        [JsonProperty]
        public string Name { get; set; } = string.Empty;

        [JsonProperty]
        public Guid GUID { get; set; } = new Guid();

        [JsonProperty]
        public SortedSet<Guid> MusicFiles { get; set; } = new SortedSet<Guid>();

        public Playlist(string Name) : base()
        {
            this.Name = Name;
        }
    }
}
