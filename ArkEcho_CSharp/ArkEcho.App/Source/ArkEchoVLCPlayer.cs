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
            LibVLCSharp.Shared.Core.Initialize();
            libvlc = new LibVLC(enableDebugLogs: true);
            mediaplayer = new MediaPlayer(libvlc);
        }

        public bool Playing { get { return mediaplayer.IsPlaying; } }

        protected override bool InitImpl(MusicFile File)
        {
            try
            {
                mediaplayer.Media = new Media(libvlc, File.LocalFileName);
                return mediaplayer.Media != null;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        protected override bool PlayImpl()
        {
            return mediaplayer.Play();
        }

        protected override bool PauseImpl()
        {
            mediaplayer.Pause();
            return mediaplayer.IsPlaying;
        }

        protected override bool StopImpl()
        {
            mediaplayer.Stop();
            return mediaplayer.IsPlaying;
        }
    }
}
