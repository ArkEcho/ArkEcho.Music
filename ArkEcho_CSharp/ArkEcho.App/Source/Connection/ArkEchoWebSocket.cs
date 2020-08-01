using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace arkecho_app.source.connection
{
    public static class ArkEchoWebSocket
    {
        private static Websockets.IWebSocketConnection socket_;
        private static bool failed_;

        public delegate void WebSocketDelegateMessage(string message);
        public static event WebSocketDelegateMessage newMessageReceived;

        public delegate void WebSocketDelegateClosed();
        public static event WebSocketDelegateClosed webSocketConnectionClosed;

        static ArkEchoWebSocket()
        {
            socket_ = Websockets.WebSocketFactory.Create();
            socket_.OnMessage += onWebSocketMessage;
            socket_.OnError += onWebSocketError;
            socket_.OnClosed += onWebSocketClosed;
        }

        private static void emitNewMessageReceived(string message)
        {
            if (newMessageReceived != null) newMessageReceived(message);
        }

        private static void emitWebSocketConnectionClosed()
        {
            if (webSocketConnectionClosed != null) webSocketConnectionClosed();
        }

        public static async Task connectWebSocket(string address)
        {
            failed_ = false;
            timeOut();

            socket_.Open("ws://" + address);

            while (!socket_.IsOpen && !failed_)
            {
                await Task.Delay(10);
            }
        }

        public static void sendMessage(int messageType, string message)
        {
            if (!socket_.IsOpen) return;
            socket_.Send(MessageHandler.createMessage(messageType, message));
        }

        public static bool checkIfConnectionIsOpen()
        {
            return socket_.IsOpen;
        }
        
        public static bool checkIfURIAddressIsCorrect(string address)
        {
            if (address == "") return false;
            var regex = @"[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,2}\:[0-9]{4}";
            return Regex.Match(address, regex).Success;
        }

        public static void disconnectWebSocket()
        {
            socket_.Close();
        }

        private static async void timeOut()
        {
            await Task.Delay(2000);
            failed_ = true;
        }

        private static void onWebSocketMessage(string message)
        {
            emitNewMessageReceived(message);
        }

        private static void onWebSocketError(string error)
        {
            failed_ = true;
        }

        private static void onWebSocketClosed()
        {
            emitWebSocketConnectionClosed();
        }
    }
}