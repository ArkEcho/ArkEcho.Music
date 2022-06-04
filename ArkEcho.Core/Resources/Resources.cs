using System.Collections.Generic;

namespace ArkEcho
{
    public class Resources
    {
        public static readonly List<string> SupportedMusicFileFormats = new List<string>()
        {
            "mp3",
            "m4a",
            "wma"
        };

        public static readonly List<string> SupportedPlaylistFileFormats = new List<string>()
        {
            "wpl", // Windows Media Player
        };
    }
}
