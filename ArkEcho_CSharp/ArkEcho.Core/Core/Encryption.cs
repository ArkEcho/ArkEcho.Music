using System;
using System.Security.Cryptography;
using System.Text;

namespace ArkEcho.Core
{
    public class Encryption
    {
        public static string Encrypt(string password)
        {
            var provider = MD5.Create();
            string salt = "S0m3R@nd0mSaltmoR3";
            byte[] bytes = provider.ComputeHash(Encoding.UTF32.GetBytes(salt + password));
            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }
    }
}
