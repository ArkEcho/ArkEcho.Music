using ArkEcho.Core;
using System;
using System.Threading.Tasks;

namespace ArkEcho.WebPage
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
            // TODO
            //string lib = await ArkEchoServer.Instance.GetMusicLibraryString();
            return true;//await Library.LoadFromJsonString(lib);
        }

        public User CheckUserForLogin(User user)
        {
            // TODO
            return new User();//ArkEchoServer.Instance.CheckUserForLogin(user);
        }
    }
}
