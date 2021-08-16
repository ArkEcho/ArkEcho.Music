using ArkEcho.Core;
using LibVLCSharp.Shared;
using System;

namespace ArkEcho.Player
{
    public class ArkEchoVLCPlayer : ArkEchoPlayer
    {
        private LibVLC libvlc = null;
        private MediaPlayer mediaplayer = null;

        public ArkEchoVLCPlayer() : base()
        {
        }

        protected override bool initPlayerImpl()
        {
            try
            {
                LibVLCSharp.Shared.Core.Initialize();
                libvlc = new LibVLC(enableDebugLogs: true);
                mediaplayer = new MediaPlayer(libvlc);
                return true;
            }
            catch(Exception ex)
            {
                log(ex.Message, Resources.LogLevel.Error);
                return false;
            }
        }

        protected override void loadImpl(bool StartOnLoad)
        {
            try
            {
                MusicFile File = PlayingFile;
                if(File != null)
                    mediaplayer.Media = new Media(libvlc, File.LocalFileName);

                if (StartOnLoad)
                    mediaplayer.Play();
            }
            catch (Exception ex)
            {
                log(ex.Message, Resources.LogLevel.Error);
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
            if(mediaplayer.Media != null)
                mediaplayer.Position = NewPosition / mediaplayer.Media.Duration;
        }
    }
}
