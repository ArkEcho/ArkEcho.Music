namespace ArkEcho
{
    public static class Logging
    {
        public enum LogLevel
        {
            Static = 0,
            Error,
            Important,
            Debug,
        }

        public delegate bool LoggingDelegate(string Text, LogLevel Level);
    }
}
