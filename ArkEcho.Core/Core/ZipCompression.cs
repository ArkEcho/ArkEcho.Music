using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;

namespace ArkEcho.Core
{
    public static class ZipCompression
    {
        private static async Task CopyTo(Stream src, Stream dest)
        {
            byte[] bytes = new byte[4096];
            int cnt = 0;

            while ((cnt = await src.ReadAsync(bytes, 0, bytes.Length)) != 0)
                await dest.WriteAsync(bytes, 0, cnt);
        }

        public static async Task<string> ZipToBase64(string unzipped)
        {
            byte[] arr = await Zip(unzipped.GetByteArray());
            return Convert.ToBase64String(arr);
        }

        public static async Task<byte[]> Zip(byte[] array)
        {
            using (MemoryStream inStream = new MemoryStream(array))
            {
                using (MemoryStream outStream = new MemoryStream())
                {
                    using (GZipStream gs = new GZipStream(outStream, CompressionMode.Compress))
                        await CopyTo(inStream, gs);

                    return outStream.ToArray();
                }
            }
        }

        public static async Task<string> UnzipBase64(string zippedBase64)
        {
            byte[] arr = await Unzip(Convert.FromBase64String(zippedBase64));
            return Encoding.Default.GetString(arr);
        }

        public static async Task<byte[]> Unzip(byte[] zipped)
        {
            using (MemoryStream inStream = new MemoryStream(zipped))
            {
                using (MemoryStream outStream = new MemoryStream())
                {
                    using (GZipStream gs = new GZipStream(inStream, CompressionMode.Decompress))
                        await CopyTo(gs, outStream);

                    return outStream.ToArray();
                }
            }
        }
    }
}
