using ArkEcho.Core;
using Microsoft.JSInterop;
using System;

namespace ArkEcho.WebPage
{
    public class JSPlayer : Player
    {
        private Logging.LoggingDelegate logDelegate = null;
        private string serverAddress = string.Empty;

        public IJSRuntime JS { get; private set; } = null;

        public JSPlayer(string serverAddress) : base()
        {
            this.serverAddress = serverAddress;
        }

        public bool InitPlayer(IJSRuntime JS)
        {
            if (JS == null)
                return false;

            if (Initialized)
                return Initialized;

            logDelegate = logConsole;
            this.JS = JS;

            var dotNetReference = DotNetObjectReference.Create(this);
            JS.InvokeVoidAsync("Player.Init", new object[] { dotNetReference });

            Initialized = true;
            return Initialized;
        }

        private bool logConsole(string text, Logging.LogLevel level)
        {
            Console.WriteLine(text);
            return true;
        }

        [JSInvokable]
        public void SetPositionJS(int Position)
        {
            base.Position = Position;
        }

        [JSInvokable]
        public void AudioEndedJS()
        {
            AudioEnd();
        }

        [JSInvokable]
        public void AudioPlayingJS(bool Playing)
        {
            this.Playing = Playing;
        }

        protected override bool log(string Text, Logging.LogLevel Level)
        {
            if (logDelegate != null)
                return logDelegate.Invoke(Text, Level);
            return false;
        }

        protected override void loadAudio(bool StartOnLoad)
        {
            string source = $"{serverAddress}/api/Music/{PlayingFile.GUID}";
            JS.InvokeVoidAsync("Player.InitAudio", new object[] { source, PlayingFile.FileFormat, StartOnLoad, Volume, Mute });
        }

        protected override void disposeAudio()
        {
            JS.InvokeVoidAsync("Player.DisposeAudio", new object[] { });
        }

        protected override void playAudio()
        {
            JS.InvokeVoidAsync("Player.PlayAudio", new object[] { });
        }

        protected override void pauseAudio()
        {
            JS.InvokeVoidAsync("Player.PauseAudio", new object[] { });
        }

        protected override void stopAudio()
        {
            JS.InvokeVoidAsync("Player.StopAudio", new object[] { });
        }

        protected override void setAudioMute()
        {
            JS.InvokeVoidAsync("Player.SetAudioMute", new object[] { Mute });
        }

        protected override void setAudioVolume()
        {
            JS.InvokeVoidAsync("Player.SetAudioVolume", new object[] { Volume });
        }

        protected override void setAudioPosition()
        {
            JS.InvokeVoidAsync("Player.SetAudioPosition", new object[] { Position });
        }
    }
}
