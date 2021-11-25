using System;
using System.Text;

namespace ArkEcho
{
    public static class EM_byteArray
    {
        public static string ToBase64(this byte[] array)
        {
            return Convert.ToBase64String(array);
        }

        public static string GetString(this byte[] array)
        {
            return Encoding.Default.GetString(array);
        }
    }
}
