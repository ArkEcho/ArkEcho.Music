using System;

namespace ArkEcho.Core
{
    public static class EM_ConvertTime
    {
        public static string ConvertTimeMilliseconds(this int value)
        {
            return ConvertTimeMilliseconds((long)value);
        }

        public static string ConvertTimeMilliseconds(this long value)
        {
            return TimeSpan.FromMilliseconds(value).ToString(@"mm\:ss");
        }

        public static string ConvertTimeSeconds(this int value)
        {
            return ConvertTimeSeconds((long)value);
        }

        public static string ConvertTimeSeconds(this long value)
        {
            return TimeSpan.FromSeconds(value).ToString(@"mm\:ss");
        }
    }
}
