using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace ArkEcho.Core
{
    public static class ZipCompression
    {
        // TODO: Async
        private static void CopyTo(Stream src, Stream dest)
        {
            byte[] bytes = new byte[4096];

            int cnt;

            while ((cnt = src.Read(bytes, 0, bytes.Length)) != 0)
                dest.Write(bytes, 0, cnt);
        }

        public static string ZipStringToBase64(string unzipped)
        {
            return Convert.ToBase64String(ZipStringToByteArray(unzipped));
        }

        public static byte[] ZipStringToByteArray(string unzipped)
        {
            return ZipByteArray(Encoding.UTF8.GetBytes(unzipped));
        }

        public static byte[] ZipByteArray(byte[] array)
        {
            using (MemoryStream msi = new MemoryStream(array))
            using (MemoryStream mso = new MemoryStream())
            {
                using (GZipStream gs = new GZipStream(mso, CompressionMode.Compress))
                {
                    //msi.CopyTo(gs);
                    CopyTo(msi, gs);
                }

                return mso.ToArray();
            }
        }

        // TODO: Bessere Funktionsnamen mit UTF8, Base64 und Zip etc.
        public static string UnzipFromBase64(string zippedBase64)
        {
            return UnzipFromByteArray(Convert.FromBase64String(zippedBase64));
        }

        public static string UnzipFromByteArray(byte[] zipped)
        {
            return Encoding.UTF8.GetString(UnzipByteArray(zipped));
        }

        public static byte[] UnzipByteArray(byte[] zipped)
        {
            using (MemoryStream msi = new MemoryStream(zipped))
            using (MemoryStream mso = new MemoryStream())
            {
                using (GZipStream gs = new GZipStream(msi, CompressionMode.Decompress))
                {
                    //gs.CopyTo(mso);
                    CopyTo(gs, mso);
                }

                return mso.ToArray();
            }
        }
    }
}
