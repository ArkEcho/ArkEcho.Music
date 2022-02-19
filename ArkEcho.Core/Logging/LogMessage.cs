using System;

namespace ArkEcho.Core
{
    public enum LogLevel
    {
        Static = 0,
        Error,
        Important,
        Debug,
    }

    public static class LogStatic
    {

    }

    public class LogMessage : JsonBase
    {
        [JsonProperty]
        public Logger Origin { get; set; }

        [JsonProperty]
        public DateTime TimeStamp { get; set; }

        [JsonProperty]
        public LogLevel Level { get; set; }

        [JsonProperty]
        public string Message { get; set; }

        public string ToLogString()
        {
            return $"{TimeStamp} - {shortLevel()}\\{contextWithPoints()}: {Message}";
        }

        private string contextWithPoints()
        {
            string points = string.Empty;
            for (int i = 8 - Origin.Context.Length; i > 0; i--) points += ".";
            return Origin.Context + points;
        }

        private string shortLevel()
        {
            switch (Level)
            {
                case LogLevel.Static:
                    return "S";
                case LogLevel.Error:
                    return "E";
                case LogLevel.Important:
                    return "I";
                case LogLevel.Debug:
                    return "D";
            }
            return "__ERROR__";
        }
    }
}
