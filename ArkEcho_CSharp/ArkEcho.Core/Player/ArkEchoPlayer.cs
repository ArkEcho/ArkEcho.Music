using ArkEcho.Core;
using System;
using System.Collections.Generic;

namespace ArkEcho.Player
{
    // TODO: JSONBase -> Einstellungen nach Nutzer speichern
    public abstract class ArkEchoPlayer
    {
        public List<MusicFile> ListToPlay { get; private set; } = null;
        public int Position { get; private set; }

        public MusicFile PlayingFile
        {
            get { return ListToPlay != null ? ListToPlay.Count > Position && Position >= 0 ? ListToPlay[Position] : null : null; }
        }


        public event Action TitleChanged;

        /// <summary>
        /// Volume, 0 - 100
        /// </summary>
        public int Volume { get { return volume; } set { volume = value; setVolumeImpl(); } }
        private int volume = 50;

        public bool Mute { get { return muted; } set { muted = value; setMuteImpl(); } }
        private bool muted = false;

        public bool Shuffle { get; set; } = false;

        public ArkEchoPlayer()
        {
        }

        public void Start(List<MusicFile> MusicFiles, int PositionToStart)
        {
            // TODO: Liste und Position während wiedergabe ändern? -> Playlist starten, dann anders ordnen und trotzdem den nächsten Abspielen
            ListToPlay = MusicFiles;
            Position = PositionToStart;

            load(true);
        }

        private void load(bool StartOnLoad)
        {
            disposeImpl();

            loadImpl(StartOnLoad);

            TitleChanged?.Invoke();
        }

        public void Play()
        {
            playImpl();
        }

        public void Pause()
        {
            pauseImpl();
        }

        public void PlayPause()
        {
            playPauseImpl();
        }

        public void Stop()
        {
            stopImpl();
        }

        public void Forward()
        {
            Position++;
            if (Position == ListToPlay.Count)
            {
                Position = 0;
                load(false);
            }
            else
                load(true);
        }

        public void Backward()
        {
            // TODO: Set Audio to 0 (Stop and Play), if double klicked backward
        }

        public void AudioEnd()
        {
            // TODO: Load next MusicFile
        }

        protected abstract void loadImpl(bool StartOnLoad);
        protected abstract void disposeImpl();
        protected abstract void playImpl();
        protected abstract void pauseImpl();
        protected abstract void playPauseImpl();
        protected abstract void stopImpl();
        protected abstract void setMuteImpl();
        protected abstract void setVolumeImpl();
    }
}
