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

        public ArkEchoRest Rest { get; private set; } = null;

        public AppModel()
        {
            Counter = 0;
            Library = new MusicLibrary();
            Rest = new ArkEchoRest("https://192.168.178.20:5002", false);
        }

        public async Task<bool> Initialize()
        {
            string lib = await Rest.GetMusicLibrary();
            return await Library.LoadFromJsonString(lib);
        }
    }
}
