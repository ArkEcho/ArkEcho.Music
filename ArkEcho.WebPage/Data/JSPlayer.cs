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
            this.apiToken = apiToken;
            try
            {
                var dotNetReference = DotNetObjectReference.Create(this);
                jsRuntime.InvokeVoidAsync("Player.Init", new object[] { dotNetReference });
                return true;
            }
            catch { return false; }
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

        [JSInvokable]
        public void BrowserPlayPause()
        {
            PlayPause();
        }

        [JSInvokable]
        public void BrowserStop()
        {
            Stop();
        }

        [JSInvokable]
        public void BrowserPreviousTrack()
        {
            Backward();
        }

        [JSInvokable]
        public void BrowserNextTrack()
        {
            Forward();
        }

        protected override void log(string text, Logging.LogLevel level)
        {
            logger.Log(text, level);
        }

        protected override void loadAudio(bool StartOnLoad)
        {
            string source = $"{serverAddress}/api/Music?{Resources.UrlParamMusicFile}={PlayingFile.GUID}&{Resources.UrlParamApiToken}={apiToken}"; // Howler doesn't support adding HTML5 Header
            string pageTitle = $"{PlayingFile.Title} - {PlayingFile.Performer}";
            jsRuntime.InvokeVoidAsync("Player.SetDocumentTitle", new object[] { pageTitle });
            jsRuntime.InvokeVoidAsync("Player.InitAudio", new object[] { source, StartOnLoad, Volume, Mute });
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
            //jsRuntime.InvokeVoidAsync("Player.StopAudio", new object[] { });
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

        protected override void dispose()
        {
            jsRuntime.InvokeVoidAsync("Player.SetDocumentTitle", new object[] { "ArkEcho" });
        }
    }
}
