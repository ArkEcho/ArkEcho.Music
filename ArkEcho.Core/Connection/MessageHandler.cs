using Newtonsoft.Json.Linq;

namespace ArkEcho.Core.Connection
{
    public class MessageHandler
    {
        private const string JSON_TYPE = "Type";
        private const string JSON_MESSAGE = "Message";

        private const string JSON_COVER_ART = "CoverArt";
        private const string JSON_SONG_TITLE = "SongTitle";
        private const string JSON_SONG_INTERPRET = "SongInterpret";
        private const string JSON_ALBUM_TITLE = "AlbumTitle";
        private const string JSON_ALBUM_INTERPRET = "AlbumInterpret";

        public enum MessageTypes_WS
        {
            NotDefined = 0,
            Echo,
            Play_Pause,
            Backward,
            Forward,
            RequestSong,
            SendSong
        }

        public static string createMessage(int messageType, string message)
        {
            JObject obj = new JObject
            {
                [JSON_TYPE] = messageType,
                [JSON_MESSAGE] = message
            };
            return obj.ToString();
        }

        public static int handleReceivedMessage(ref string message)
        {
            JObject obj = JObject.Parse(message);
            int messageType = obj[JSON_TYPE].ToObject<int>();
            message = obj[JSON_MESSAGE].ToObject<string>();
            return messageType;
        }

        //public static Bitmap base64ToImage(string base64String)
        //{
        //    byte[] decodedBytes = Base64.Decode(base64String, 0);
        //    return BitmapFactory.DecodeByteArray(decodedBytes, 0, decodedBytes.Length);
        //}
    }
}