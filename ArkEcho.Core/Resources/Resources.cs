﻿using System;
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
        public const int RestChunkSize = 1048576;// 1mb //15728640; // 15mb
        public const int RestMusicFileCount = 500; // Only 500 Objects are transmitted at once

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

        // WindowsApiCodePack Shell Properties Rating Values
        public enum Rating : byte
        {
            None = 0,
            One = 1,
            Two = 2,
            Three = 3,
            Four = 4,
            Five = 5,
        }

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
            else if (osDescription.Contains("Linux"))
                return "/";
            else
                throw new Exception($"Unknown OS! OSDescription: {osDescription}");
        }
    }
}
