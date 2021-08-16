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
            throw new NotImplementedException();
        }

        protected override void pauseImpl()
        {
            throw new NotImplementedException();
        }

        protected override void stopImpl()
        {
            throw new NotImplementedException();
        }

        protected override void setMuteImpl()
        {
            throw new NotImplementedException();
        }

        protected override void setVolumeImpl()
        {
            throw new NotImplementedException();
        }

        protected override void setPositionImpl(int NewPosition)
        {
            throw new NotImplementedException();
        }
    }
}
