using ArkEcho.Core;
using LibVLCSharp.Shared;
using System;

namespace ArkEcho.Player
{
    public class ArkEchoVLCPlayer : ArkEchoPlayer
    {
        private LibVLC libvlc = null;
        private MediaPlayer mediaplayer = null;
        private Resources.LoggingDelegate logDelegate = null;

        public ArkEchoVLCPlayer() : base()
        {
        }

        protected override bool logImpl(string Text, Resources.LogLevel Level)
        {
            if (logDelegate != null)
                return logDelegate.Invoke(Text, Level);
            return false;
        }

        public bool InitPlayer(Resources.LoggingDelegate LogDelegate)
        {
            try
            {
                if (LogDelegate != null)
                {
                    logDelegate = LogDelegate;

                    LibVLCSharp.Shared.Core.Initialize();
                    libvlc = new LibVLC(enableDebugLogs: true);
                    mediaplayer = new MediaPlayer(libvlc);

                    Initialized = true;
                    return Initialized;
                }
                return false;
            }
            catch (Exception ex)
            {
                logImpl(ex.Message, Resources.LogLevel.Error);
                return false;
            }
        }

        protected override void loadImpl(bool StartOnLoad)
        {
            try
            {
                MusicFile File = PlayingFile;
                if (File != null)
                    mediaplayer.Media = new Media(libvlc, File.GetFullPathAndroid());

                if (StartOnLoad)
                    mediaplayer.Play();
            }
            catch (Exception ex)
            {
                logImpl(ex.Message, Resources.LogLevel.Error);
            }
        }

        protected override void disposeImpl()
        {
            mediaplayer.Media?.Dispose();
        }

        protected override void playImpl()
        {
            mediaplayer.Play();
        }

        protected override void pauseImpl()
        {
            mediaplayer.Pause();
        }

        protected override void stopImpl()
        {
            mediaplayer.Stop();
        }

        protected override void setMuteImpl()
        {
            mediaplayer.Mute = Mute;
        }

        protected override void setVolumeImpl()
        {
            mediaplayer.Volume = Volume;
        }

        protected override void setPositionImpl(int NewPosition)
        {
            if (mediaplayer.Media != null)
                mediaplayer.Position = NewPosition / mediaplayer.Media.Duration;
        }
    }
}
