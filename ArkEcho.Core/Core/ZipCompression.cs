using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace ArkEcho.Core
{
    public static class ZipCompression
    {
        private static void CopyTo(Stream src, Stream dest)
        {
            byte[] bytes = new byte[4096];

            int cnt;

            while ((cnt = src.Read(bytes, 0, bytes.Length)) != 0)
                dest.Write(bytes, 0, cnt);
        }

        public static string ZipToBase64(string unzipped)
        {
            return Convert.ToBase64String(ZipToByteArray(unzipped));
        }

        public static byte[] ZipToByteArray(string unzipped)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(unzipped);

            using (MemoryStream msi = new MemoryStream(bytes))
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

        public static string UnzipFromBase64(string zipped)
        {
            return UnzipFromByteArray(Convert.FromBase64String(zipped));
        }

        public static string UnzipFromByteArray(byte[] zipped)
        {
            using (MemoryStream msi = new MemoryStream(zipped))
            using (MemoryStream mso = new MemoryStream())
            {
                using (GZipStream gs = new GZipStream(msi, CompressionMode.Decompress))
                {
                    //gs.CopyTo(mso);
                    CopyTo(gs, mso);
                }

                return Encoding.UTF8.GetString(mso.ToArray());
            }
        }

    }
}
