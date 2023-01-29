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
            string format = value > 3600 ? @"hh\:mm\:ss" : @"mm\:ss";
            return TimeSpan.FromMilliseconds(value).ToString(format);
        }

        public static string ConvertTimeSeconds(this int value)
        {
            return ConvertTimeSeconds((long)value);
        }

        public static string ConvertTimeSeconds(this long value)
        {
            string format = value > 3600 ? @"hh\:mm\:ss" : @"mm\:ss";
            return TimeSpan.FromSeconds(value).ToString(format);
        }
    }
}
