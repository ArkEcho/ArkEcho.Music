using System.Text.Json;
using System.Threading.Tasks;

namespace ArkEcho.Core
{
    public class Serializer
    {
        public static async Task<byte[]> Serialize<T>(T obj)
        {
            byte[] result = null;
            await Task.Run(() =>
            {
                result = JsonSerializer.SerializeToUtf8Bytes<T>(obj);
            });
            return result;
        }

        public static async Task<T> Deserialize<T>(byte[] data)
        {
            T result = default(T);

            await Task.Run(() =>
            {
                result = JsonSerializer.Deserialize<T>(data);
            });

            return result;
        }
    }
}
