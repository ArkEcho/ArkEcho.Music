using Newtonsoft.Json.Linq;
using Android.Graphics;
using Android.Util;

namespace arkecho_app.source.connection
{
    class MessageHandler
    {
        private const string JSON_TYPE = "Type";
        private const string JSON_MESSAGE = "Message";

        public enum MESSAGETYPE
        {
            MT_NOTDEFINED = 0,
            MT_ECHO_TEST,
            MT_PLAY_PAUSE,
            MT_BACKWARD,
            MT_FORWARD,
            MT_REQUEST_SONG_ACTUAL,
            MT_SEND_SONG_ACTUAL
        }

        public static string createMessage(int messageType, string message)
        {
            JObject obj = new JObject();
            obj[JSON_TYPE] = messageType;
            obj[JSON_MESSAGE] = message;
            return obj.ToString();
        }

        public static int handleReceivedMessage(ref string message)
        {
            JObject obj = JObject.Parse(message);
            int messageType = obj[JSON_TYPE].ToObject<int>();
            message = obj[JSON_MESSAGE].ToObject<string>();
            return messageType;
        }

        public static Bitmap base64ToImage(string base64String)
        {
            byte[] decodedBytes = Base64.Decode(base64String, 0);
            return BitmapFactory.DecodeByteArray(decodedBytes, 0, decodedBytes.Length);
        }
    }
}