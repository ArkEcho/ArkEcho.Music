using System;
using System.Security.Cryptography;
using System.Text;

namespace ArkEcho.Core
{
    public class Encryption
    {
        public static string EncryptMD5(string password)
        {
            var provider = MD5.Create();
            string salt = "S0m3R@nd0mSaltmoR3";
            byte[] bytes = provider.ComputeHash(Encoding.UTF32.GetBytes(salt + password));
            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }

        public static string EncryptSHA256(string text)
        {
            byte[] bytes = Encoding.Default.GetBytes(text);
            return EncryptSHA256(bytes);
        }

        public static string EncryptSHA256(byte[] bytes)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] data = sha256Hash.ComputeHash(bytes);

                StringBuilder sBuilder = new StringBuilder();
                for (int i = 0; i < data.Length; i++)
                    sBuilder.Append(data[i].ToString("x2"));

                return sBuilder.ToString();
            }
        }
    }
}
