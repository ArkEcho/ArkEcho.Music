using ArkEcho.Core;
using LibVLCSharp.Shared;
using System;

namespace ArkEcho.App
{
    // TODO: Entfernen durch RazorPages Klasse
    public class VLCPlayer : Player
    {
        private LibVLC libvlc = null;
        private MediaPlayer mediaplayer = null;
        public VLCPlayer() : base()
        {
        }

        protected override bool log(string Text, Logging.LogLevel Level)
        {
            // TODO Logger
            return false;
        }

        public bool InitPlayer()
        {
            try
            {
                LibVLCSharp.Shared.Core.Initialize();
                libvlc = new LibVLC(enableDebugLogs: true);
                mediaplayer = new MediaPlayer(libvlc);
                mediaplayer.PositionChanged += Mediaplayer_PositionChanged;
                mediaplayer.EndReached += Mediaplayer_EndReached;

                Initialized = true;
                return Initialized;
            }
            catch (Exception ex)
            {
                log(ex.Message, Logging.LogLevel.Error);
                return false;
            }
        }

        private void Mediaplayer_EndReached(object sender, EventArgs e)
        {
            AudioEnd();
        }

        private void Mediaplayer_PositionChanged(object sender, MediaPlayerPositionChangedEventArgs e)
        {
            // TODO: Position in Sekunden berechnen
            //audioPositionChanged((PlayingFile.Duration / 1000) * mediaplayer.Position);
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
        }

        protected override void playAudio()
        {
            mediaplayer.Play();
        }

        protected override void pauseAudio()
        {
            mediaplayer.Pause();
        }

        protected override void stopAudio()
        {
            mediaplayer.Stop();
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
            if (mediaplayer.Media != null)
                mediaplayer.Position = Position / mediaplayer.Media.Duration;
        }
    }
}
