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

        protected override void InitImpl(MusicFile File)
        {
            string source = $"https://localhost:5001/api/Music/MusicFile/{File.GUID}";
            JS?.InvokeVoidAsync("InitAudio", new object[] { source, File.FileFormat });
        }

        protected override void DisposeImpl()
        {
            JS?.InvokeVoidAsync("DisposeAudio", new object[] { });
        }

        protected override void PlayImpl()
        {
            JS?.InvokeVoidAsync("PlayAudio", new object[] { });
        }

        protected override void PauseImpl()
        {
            JS?.InvokeVoidAsync("PauseAudio", new object[] { });
        }
        protected override void PlayPauseImpl()
        {
            JS?.InvokeVoidAsync("PlayPauseAudio", new object[] { });
        }

        protected override void StopImpl()
        {
            JS?.InvokeVoidAsync("StopAudio", new object[] { });
        }

        protected override void SetMuteImpl(bool Mute)
        {
            JS?.InvokeVoidAsync("SetAudioMute", new object[] { Mute });
        }

        protected override void SetVolumeImpl(int NewVolume)
        {
            JS?.InvokeVoidAsync("SetAudioVolume", new object[] { NewVolume });
        }
    }
}
