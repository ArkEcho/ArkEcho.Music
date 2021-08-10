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

        protected override void loadImpl(bool StartOnLoad)
        {
            // TODO: Adresse dynamisch
            MusicFile file = PlayingFile;
            if (file != null)
            {
                string source = $"https://localhost:5001/api/Music/MusicFile/{file.GUID}";
                JS?.InvokeVoidAsync("InitAudio", new object[] { source, file.FileFormat, StartOnLoad, Volume, Mute });
            }
        }

        protected override void disposeImpl()
        {
            JS?.InvokeVoidAsync("DisposeAudio", new object[] { });
        }

        protected override void playImpl()
        {
            JS?.InvokeVoidAsync("PlayAudio", new object[] { });
        }

        protected override void pauseImpl()
        {
            JS?.InvokeVoidAsync("PauseAudio", new object[] { });
        }
        protected override void playPauseImpl()
        {
            JS?.InvokeVoidAsync("PlayPauseAudio", new object[] { });
        }

        protected override void stopImpl()
        {
            JS?.InvokeVoidAsync("StopAudio", new object[] { });
        }

        protected override void setMuteImpl()
        {
            JS?.InvokeVoidAsync("SetAudioMute", new object[] { Mute });
        }

        protected override void setVolumeImpl()
        {
            JS?.InvokeVoidAsync("SetAudioVolume", new object[] { Volume });
        }
    }
}
