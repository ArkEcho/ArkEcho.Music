using ArkEcho.Core;
using System;
using System.Threading.Tasks;

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

        public async Task<bool> Initialize()
        {
            string lib = await ArkEchoServer.Instance.GetMusicLibraryString();
            return await Library.LoadFromJsonString(lib);
        }

        public User CheckUserForLogin(User user)
        {
            return ArkEchoServer.Instance.CheckUserForLogin(user);
        }
    }
}
