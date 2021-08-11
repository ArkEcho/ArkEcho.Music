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

        public void Init(IJSRuntime JS)
        {
            this.JS = JS;
            var dotNetReference = DotNetObjectReference.Create(this);
            JS?.InvokeVoidAsync("Player.Init", new object[] { dotNetReference });
        }

        [JSInvokable]
        public void SetPosition(int Position)
        {
            base.Position = Position;
        }


        [JSInvokable]
        public void AudioEnded()
        {
            AudioEnd();
        }


        protected override void loadImpl(bool StartOnLoad)
        {
            // TODO: Adresse dynamisch
            MusicFile file = PlayingFile;
            if (file != null)
            {
                string source = $"https://localhost:5001/api/Music/MusicFile/{file.GUID}";
                JS?.InvokeVoidAsync("Player.InitAudio", new object[] { source, file.FileFormat, StartOnLoad, Volume, Mute });
            }
        }

        protected override void disposeImpl()
        {
            JS?.InvokeVoidAsync("Player.DisposeAudio", new object[] { });
        }

        protected override void playImpl()
        {
            JS?.InvokeVoidAsync("Player.PlayAudio", new object[] { });
        }

        protected override void pauseImpl()
        {
            JS?.InvokeVoidAsync("Player.PauseAudio", new object[] { });
        }
        protected override void playPauseImpl()
        {
            JS?.InvokeVoidAsync("Player.PlayPauseAudio", new object[] { });
        }

        protected override void stopImpl()
        {
            JS?.InvokeVoidAsync("Player.StopAudio", new object[] { });
        }

        protected override void setMuteImpl()
        {
            JS?.InvokeVoidAsync("Player.SetAudioMute", new object[] { Mute });
        }

        protected override void setVolumeImpl()
        {
            JS?.InvokeVoidAsync("Player.SetAudioVolume", new object[] { Volume });
        }

        protected override void setPositionImpl(int NewDuration)
        {
            JS?.InvokeVoidAsync("Player.SetAudioPosition", new object[] { NewDuration });
        }
    }
}
