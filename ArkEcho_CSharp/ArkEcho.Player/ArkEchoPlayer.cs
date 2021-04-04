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

        public void Play(string Path)
        {
            Media media = new Media(libvlc, Path);

            mediaplayer.Play(media);
        }

        public void Stop()
        {
            mediaplayer.Stop();
        }
    }
}
