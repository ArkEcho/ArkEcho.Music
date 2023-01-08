using System;
using System.Text;

namespace ArkEcho
{
    public static class EM_String
    {
        public static string RemoveCRLF(this string inputString)
        {
            return inputString.Replace("\r\n", "").Replace("\n", "");
        }

        public static string EscapeString(this string inputString)
        {
            return inputString.Replace("\\", "\\\\");
        }
        public static string UnEscapeString(this string inputString)
        {
            return inputString.Replace("\\\\", "\\");
        }

        public static string ToBase64(this string inputString)
        {
            return Convert.ToBase64String(Encoding.Default.GetBytes(inputString));
        }

        public static string FromBase64(this string inputString)
        {
            return Encoding.Default.GetString(Convert.FromBase64String(inputString));
        }

        public static byte[] GetByteArray(this string inputString)
        {
            return Encoding.Default.GetBytes(inputString);
        }
        public static string? Truncate(this string value, int maxLength, string truncationSuffix = "…")
        {
            return value?.Length > maxLength
                ? value.Substring(0, maxLength - truncationSuffix.Length - 2) + truncationSuffix
                : value;
        }
    }
}
