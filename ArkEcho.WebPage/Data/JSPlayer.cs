using ArkEcho.Core;
using Microsoft.JSInterop;

namespace ArkEcho.WebPage
{
    public class JSPlayer : Player
    {
        private string serverAddress = string.Empty;

        private IJSRuntime jsRuntime = null;
        private Logger logger = null;
        private AppEnvironment appEnvironment;
        private Rest rest;
        private bool initialized = false;

        public JSPlayer(IJSRuntime jsRuntime, Logger logger, Rest rest, AppEnvironment appEnvironment) : base()
        {
            this.jsRuntime = jsRuntime;
            this.logger = logger;
            this.appEnvironment = appEnvironment;
            this.rest = rest;
        }

        public override bool InitializePlayer()
        {
            if (initialized)
                return true;

            try
            {
                var dotNetReference = DotNetObjectReference.Create(this);
                jsRuntime.InvokeVoidAsync("Player.Init", new object[] { dotNetReference });
                initialized = true;
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
        public void AudioFatalErrorJS(string error)
        {
            // TODO
            Console.WriteLine("JSPlayer: " + error);
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

        protected override async void loadAudio(bool StartOnLoad)
        {
            jsRuntime.InvokeVoidAsync("Player.SetDocumentTitle", new object[] { $"{PlayingFile.Title} - {PlayingFile.Performer}" });

            // Load Audio in Chunks in js File (FASTER)
            string[] chunkSources = new string[PlayingFile.Chunks.Count];

            // Also set complete Audio Source, in case Chunk loading fail... (Anti Virus)
            string completeSource = $"{serverAddress}/api/Music?{Resources.UrlParamMusicFile}={PlayingFile.GUID}&{Resources.UrlParamApiToken}={rest.ApiToken}";

            for (int i = 0; i < PlayingFile.Chunks.Count; i++)
            {
                string source = $"{serverAddress}/api/File/ChunkTransfer?{Resources.UrlParamMusicFile}={PlayingFile.GUID}&{Resources.UrlParamFileChunk}={PlayingFile.Chunks[i].GUID}&{Resources.UrlParamApiToken}={rest.ApiToken}";
                chunkSources[i] = source;
            }
            jsRuntime.InvokeVoidAsync("Player.InitAudio", new object[] { chunkSources, completeSource, PlayingFile.MimeType, StartOnLoad, Volume, Mute });
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
