using ArkEcho.Core;
using ArkEcho.RazorPage.Data;
using LibVLCSharp.Shared;

namespace ArkEcho.Maui
{
    public class VLCPlayer : Player
    {
        private LibVLC libvlc = null;
        private LibVLCSharp.Shared.MediaPlayer mediaplayer = null;
        private Logger logger = null;
        private IMauiHelper helper;
        private bool initialized = false;

        public VLCPlayer(Logger logger, IMauiHelper helper) : base()
        {
            this.logger = logger;
            this.helper = helper;
        }

        protected override void log(string Text, Logging.LogLevel Level)
        {
            logger.Log(Text, Level);
        }

        public override bool InitializePlayer()
        {
            if (initialized)
                return true;

            try
            {
                LibVLCSharp.Shared.Core.Initialize();
                libvlc = new LibVLC(enableDebugLogs: true);
                mediaplayer = new LibVLCSharp.Shared.MediaPlayer(libvlc);

                mediaplayer.PositionChanged += Mediaplayer_PositionChanged;
                mediaplayer.EndReached += Mediaplayer_EndReached;
                mediaplayer.Playing += Mediaplayer_Playing;
                mediaplayer.Paused += Mediaplayer_Paused;

                helper.MediaPlayPauseKeyPressed += () => PlayPause();
                helper.MediaPreviousTrackKeyPressed += () => Backward();
                helper.MediaNextTrackKeyPressed += () => Forward();

                setAudioVolume();
                initialized = true;
                return true;
            }
            catch (Exception ex)
            {
                log(ex.Message, Logging.LogLevel.Error);
                return false;
            }
        }

        private void Mediaplayer_Paused(object? sender, EventArgs e)
        {
            playingChanged(false);
        }

        private void Mediaplayer_Playing(object? sender, EventArgs e)
        {
            playingChanged(true);
        }

        private void Mediaplayer_EndReached(object sender, EventArgs e)
        {
            AudioEnd();
        }

        private void Mediaplayer_PositionChanged(object sender, MediaPlayerPositionChangedEventArgs e)
        {
            int position = Convert.ToInt32((double)PlayingFile.Duration / 1000 * e.Position);
            audioPositionChanged(position);
        }

        protected override void loadAudio(bool StartOnLoad)
        {
            try
            {
                MusicFile File = PlayingFile;
                if (File != null)
                    mediaplayer.Media = new Media(libvlc, File.FullPath);

                if (StartOnLoad)
                    mediaplayer.Play();
            }
            catch (Exception ex)
            {
                log(ex.Message, Logging.LogLevel.Error);
            }
        }

        protected override void disposeAudio()
        {
            mediaplayer.Media?.Dispose();
            mediaplayer.Media = null;
        }

        protected override void playAudio()
        {
            mediaplayer.Play();
        }

        protected override void pauseAudio()
        {
            mediaplayer.Pause();
        }

        protected override void setAudioMute()
        {
            mediaplayer.Mute = Mute;
        }

        protected override void setAudioVolume()
        {
            mediaplayer.Volume = Volume;
        }

        protected override void setAudioPosition()
        {
            float position = (float)Position / ((float)PlayingFile.Duration / 1000);
            mediaplayer.Position = position; // Doesn't work if not playing/stopped!
        }

        protected override void dispose()
        {
            libvlc?.Dispose();
            libvlc = null;

            mediaplayer?.Dispose();
            libvlc = null;

            initialized = false;
        }
    }
}
