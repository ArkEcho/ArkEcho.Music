using System.Threading.Tasks;
using System.Text.RegularExpressions;
using ArkEcho.Core;
using ArkEcho.Core.Connection;

namespace ArkEcho.App.Connection
{
    public static class ArkEchoWebSocket
    {
        private static Websockets.IWebSocketConnection socket;
        private static bool failed;

        public delegate void WebSocketDelegateMessage(string message);
        public static event WebSocketDelegateMessage newMessageReceived;

        public delegate void WebSocketDelegateClosed();
        public static event WebSocketDelegateClosed webSocketConnectionClosed;

        static ArkEchoWebSocket()
        {
            socket = Websockets.WebSocketFactory.Create();
            socket.OnMessage += onWebSocketMessage;
            socket.OnError += onWebSocketError;
            socket.OnClosed += onWebSocketClosed;
        }

        private static void emitNewMessageReceived(string message)
        {
            newMessageReceived?.Invoke(message);
        }

        private static void emitWebSocketConnectionClosed()
        {
            webSocketConnectionClosed?.Invoke();
        }

        public static async Task connectWebSocket(string address)
        {
            failed = false;
            timeOut();

            socket.Open("ws://" + address);

            while (!socket.IsOpen && !failed)            
                await Task.Delay(10);            
        }

        public static void sendMessage(int messageType, string message)
        {
            if (!socket.IsOpen) return;
            socket.Send(MessageHandler.createMessage(messageType, message));
        }

        public static bool checkIfConnectionIsOpen()
        {
            return socket.IsOpen;
        }
        
        public static bool checkIfURIAddressIsCorrect(string address)
        {
            if (!string.IsNullOrEmpty(address))
                return false;

            string regex = @"[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,2}\:[0-9]{4}";

            return Regex.Match(address, regex).Success;
        }

        public static void disconnectWebSocket()
        {
            socket.Close();
        }

        private static async void timeOut()
        {
            await Task.Delay(2000);
            failed = true;
        }

        private static void onWebSocketMessage(string message)
        {
            emitNewMessageReceived(message);
        }

        private static void onWebSocketError(string error)
        {
            failed = true;
        }

        private static void onWebSocketClosed()
        {
            emitWebSocketConnectionClosed();
        }
    }
}