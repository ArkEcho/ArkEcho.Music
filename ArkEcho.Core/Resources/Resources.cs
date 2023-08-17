using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace ArkEcho
{
    public class Resources
    {
        public const string PLAYLISTFOLDER = "Wiedergabelisten";

        public static readonly string FilePathDivider = getPathDiv();

        public const string ARKECHOSERVER = "ArkEcho.Server";
        public const string ARKECHOWEBPAGE = "ArkEcho.WebPage";
        public const string ARKECHOMAUI = "ArkEcho.Maui";

        public const int MaxFileSize = int.MaxValue; // ~2Gb
        public const int RestChunkSize = 1048576;// 1mb
        public const int RestMusicFileCount = 100; // Only 500 Objects are transmitted at once
        public const int ImageSize = 180;

        public const string UrlParamApiToken = "apiToken";
        public const string UrlParamSessionToken = "sessionToken";
        public const string UrlParamUserName = "userName";
        public const string UrlParamUserPassword = "userPassword";

        public const string UrlParamAlbumGuid = "albumGuid";
        public const string UrlParamMusicFile = "musicFile";
        public const string UrlParamMusicFileCountIndex = "musicFileCountIndex";
        public const string UrlParamMusicRating = "musicRating";
        public const string UrlParamFileChunk = "fileChunk";

        public enum Status
        {
            None,
            Idle,
            Busy
        }

        public enum Platform
        {
            None,
            Windows,
            Android,
            Web,
            Server
        }

        public static readonly List<string> SupportedMusicFileFormats = new List<string>()
        {
            "mp3",
            "m4a",
            //"wma" // Cant get played by Web Html Audio -> convert to MP3
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
            else if (osDescription.Contains("Linux"))
                return "/";
            else
                throw new Exception($"Unknown OS! OSDescription: {osDescription}");
        }
    }
}
