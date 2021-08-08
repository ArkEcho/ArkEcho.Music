using ArkEcho.Core;
using Microsoft.JSInterop;

namespace ArkEcho.Player
{
    public class ArkEchoJSPlayer : ArkEchoPlayer
    {
        public IJSRuntime JS { get; private set; }

        public ArkEchoJSPlayer() : base()
        {

        }

        public void SetJSRuntime(IJSRuntime JS)
        {
            this.JS = JS;
        }

        protected override bool InitImpl(MusicFile File)
        {
            JS.InvokeVoidAsync("Init", new object[] { File.GUID.ToString() });
            return true;
        }

        protected override bool PlayImpl()
        {
            JS.InvokeVoidAsync("PlayAudio", new object[] { });
            return true;
        }

        protected override bool PauseImpl()
        {
            JS.InvokeVoidAsync("PauseAudio", new object[] { });
            return true;
        }

        protected override bool StopImpl()
        {
            JS.InvokeVoidAsync("PauseAudio", new object[] { });
            return true;
        }
    }
}
