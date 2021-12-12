using System.Collections.Generic;

namespace ArkEcho
{
    public class Resources
    {
        public enum LogLevel
        {
            Static = 0,
            Error,
            Information
        }
        public delegate bool LoggingDelegate(string Text, LogLevel Level);

        public static readonly List<string> SupportedFileFormats = new List<string>()
        {
            "mp3",
            "m4a",
            "wma"
        };
    }
}
