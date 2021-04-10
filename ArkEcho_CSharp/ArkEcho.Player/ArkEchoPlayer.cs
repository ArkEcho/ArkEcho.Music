using LibVLCSharp.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace ArkEcho.Player
{
    public class ArkEchoPlayer
    {
        private LibVLC libvlc = null;
        private MediaPlayer mediaplayer = null;

        public ArkEchoPlayer()
        {
            LibVLCSharp.Shared.Core.Initialize();
            libvlc = new LibVLC(enableDebugLogs: true);
            mediaplayer = new MediaPlayer(libvlc);
        }

        public bool Playing { get { return mediaplayer.IsPlaying; } }

        public void Play(string Path)
        {
            Media media = new Media(libvlc, Path);
            mediaplayer.Play(media);
        }

        public void Pause()
        {
            mediaplayer.Pause();
        }

        public void Stop()
        {
            mediaplayer.Stop();
        }
    }
}
