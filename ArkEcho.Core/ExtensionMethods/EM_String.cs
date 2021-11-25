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

        public static byte[] FromBase64(this string inputString)
        {
            return Convert.FromBase64String(inputString);
        }

        public static byte[] GetByteArray(this string inputString)
        {
            return Encoding.Default.GetBytes(inputString);
        }
    }
}
