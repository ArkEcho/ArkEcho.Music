using ArkEcho.Core;
using Microsoft.JSInterop;
using System;

namespace ArkEcho.WebPage
{
    public class JSPlayer : Player
    {
        private Logging.LoggingDelegate logDelegate = null;
        private string serverAddress = string.Empty;

        private IJSRuntime jsRuntime = null;

        public JSPlayer(IJSRuntime jsRuntime, string serverAddress) : base()
        {
            this.jsRuntime = jsRuntime;
            this.serverAddress = serverAddress;
        }

        public bool InitPlayer()
        {
            if (Initialized)
                return Initialized;

            logDelegate = logConsole;

            var dotNetReference = DotNetObjectReference.Create(this);
            jsRuntime.InvokeVoidAsync("Player.Init", new object[] { dotNetReference });

            Initialized = true;
            return Initialized;
        }

        private bool logConsole(string text, Logging.LogLevel level)
        {
            Console.WriteLine(text);
            return true;
        }

        [JSInvokable]
        public void AudioPositionChangedJS(int Position)
        {
            audioPositionChanged(Position);
        }

        [JSInvokable]
        public void AudioEndedJS()
        {
            AudioEnd();
        }

        [JSInvokable]
        public void AudioPlayingJS(bool playing)
        {
            playingChanged(playing);
        }

        protected override bool log(string text, Logging.LogLevel level)
        {
            if (logDelegate != null)
                return logDelegate.Invoke(text, level);
            return false;
        }

        protected override void loadAudio(bool StartOnLoad)
        {
            string source = $"{serverAddress}/api/Music/{PlayingFile.GUID}";
            jsRuntime.InvokeVoidAsync("Player.InitAudio", new object[] { source, PlayingFile.FileFormat, StartOnLoad, Volume, Mute });
        }

        protected override void disposeAudio()
        {
            jsRuntime.InvokeVoidAsync("Player.DisposeAudio", new object[] { });
        }

        protected override void playAudio()
        {
            jsRuntime.InvokeVoidAsync("Player.PlayAudio", new object[] { });
        }

        protected override void pauseAudio()
        {
            jsRuntime.InvokeVoidAsync("Player.PauseAudio", new object[] { });
        }

        protected override void stopAudio()
        {
            jsRuntime.InvokeVoidAsync("Player.StopAudio", new object[] { });
        }

        protected override void setAudioMute()
        {
            jsRuntime.InvokeVoidAsync("Player.SetAudioMute", new object[] { Mute });
        }

        protected override void setAudioVolume()
        {
            jsRuntime.InvokeVoidAsync("Player.SetAudioVolume", new object[] { Volume });
        }

        protected override void setAudioPosition()
        {
            jsRuntime.InvokeVoidAsync("Player.SetAudioPosition", new object[] { Position });
        }
    }
}
