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

        protected override void InitImpl()
        {
            // TODO: Adresse dynamisch
            string source = $"https://localhost:5001/api/Music/MusicFile/{PlayingMusic.GUID}";
            JS?.InvokeVoidAsync("InitAudio", new object[] { source, PlayingMusic.FileFormat, DirectPlay, Volume, Mute });
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

        protected override void SetMuteImpl()
        {
            JS?.InvokeVoidAsync("SetAudioMute", new object[] { Mute });
        }

        protected override void SetVolumeImpl()
        {
            JS?.InvokeVoidAsync("SetAudioVolume", new object[] { Volume });
        }
    }
}
