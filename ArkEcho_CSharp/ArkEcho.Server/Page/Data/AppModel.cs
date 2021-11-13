using ArkEcho.Core;
using System;

namespace ArkEcho.Server
{
    public class AppModel
    {
        private int counter;

        public int Counter { get { return counter; } set { counter = value; CounterChanged?.Invoke(); } }

        public event Action CounterChanged;

        // TODO: private?
        public MusicLibrary Library { get; private set; } = null;

        public AppModel()
        {
            Counter = 0;
            Library = new MusicLibrary();
        }

        public bool Initialize()
        {
            return Library.LoadFromJsonString(ArkEchoServer.Instance.GetMusicLibraryString());
        }

        public User CheckUserForLogin(User user)
        {
            return ArkEchoServer.Instance.CheckUserForLogin(user);
        }
    }
}
