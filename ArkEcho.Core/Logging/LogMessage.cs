using System;

namespace ArkEcho.Core
{

    public class LogMessage : JsonBase
    {
        [JsonProperty]
        public Logger Origin { get; set; }

        [JsonProperty]
        public DateTime TimeStamp { get; set; }

        [JsonProperty]
        public Logging.LogLevel Level { get; set; }

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
                case Logging.LogLevel.Static:
                    return "S";
                case Logging.LogLevel.Error:
                    return "E";
                case Logging.LogLevel.Important:
                    return "I";
                case Logging.LogLevel.Debug:
                    return "D";
            }
            return "__ERROR__";
        }
    }
}
