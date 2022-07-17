using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace ArkEcho
{
    public class Resources
    {
        public const string PLAYLISTFOLDER = "Wiedergabelisten";

        public static readonly string FilePathDivider = getPathDiv();

        public const string ARKECHOAPP = "ArkEcho.App";
        public const string ARKECHOSERVER = "ArkEcho.Server";
        public const string ARKECHOWEBPAGE = "ArkEcho.WebPage";
        public const string ARKECHODESKTOP = "ArkEcho.Desktop";

        public static readonly List<string> SupportedMusicFileFormats = new List<string>()
        {
            "mp3",
            "m4a",
            "wma"
        };

        public static readonly List<string> SupportedPlaylistFileFormats = new List<string>()
        {
            "wpl", // Windows Media Player
            "m3u",
            "pls",
            "zpl"
        };

        private static string getPathDiv()
        {
            string osDescription = RuntimeInformation.OSDescription;
            if (osDescription.Contains("Windows", StringComparison.OrdinalIgnoreCase))
                return "\\";
            else if (osDescription.Contains("Android"))
                return "/";
            else if (osDescription.Contains("Unix") && RuntimeInformation.FrameworkDescription.Contains("Mono", StringComparison.OrdinalIgnoreCase))
                return "/";
            else
                throw new Exception($"Unknown OS! OSDescription: {osDescription}");
        }
    }
}
