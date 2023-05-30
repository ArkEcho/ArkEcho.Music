using ArkEcho.Core;
using Microsoft.JSInterop;

namespace ArkEcho.WebPage
{
    public class JSPlayer : Player
    {
        private string serverAddress = string.Empty;

        private IJSRuntime jsRuntime = null;
        private Logger logger = null;
        private string apiToken = string.Empty;

        public JSPlayer(IJSRuntime jsRuntime, Logger logger, string serverAddress) : base()
        {
            this.jsRuntime = jsRuntime;
            this.logger = logger;
            this.serverAddress = serverAddress;
        }

        public bool InitPlayer(string apiToken)
        {
            if (Initialized)
                return Initialized;

            this.apiToken = apiToken;
            var dotNetReference = DotNetObjectReference.Create(this);
            jsRuntime.InvokeVoidAsync("Player.Init", new object[] { dotNetReference });

            Initialized = true;
            return Initialized;
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

        protected override void log(string text, Logging.LogLevel level)
        {
            logger.Log(text, level);
        }

        protected override void loadAudio(bool StartOnLoad)
        {
            string source = $"{serverAddress}/api/Music?{Resources.UrlParamMusicFile}={PlayingFile.GUID}&{Resources.UrlParamApiToken}={apiToken}"; // Howler doesn't support adding HTML5 Header
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
