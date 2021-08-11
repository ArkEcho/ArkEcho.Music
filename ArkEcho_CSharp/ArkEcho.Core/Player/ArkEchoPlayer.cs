using ArkEcho.Core;
using System;
using System.Collections.Generic;

namespace ArkEcho.Player
{
    // TODO: JSONBase -> Einstellungen nach Nutzer speichern
    public abstract class ArkEchoPlayer
    {
        public List<MusicFile> ListToPlay { get; private set; } = null;
        public int SongIndex { get; private set; }

        public MusicFile PlayingFile
        {
            get
            {
                return ListToPlay != null ? ListToPlay.Count > SongIndex && SongIndex >= 0 ? ListToPlay[SongIndex] : null : null;
            }
        }


        public event Action TitleChanged;
        public event Action PositionChanged;

        /// <summary>
        /// Audio Volume, 0 - 100
        /// </summary>
        public int Volume
        {
            get { return volume; }
            set
            {
                volume = value;
                setVolumeImpl();
            }
        }
        private int volume = 50;

        /// <summary>
        /// Audio Muted
        /// </summary>
        public bool Mute
        {
            get { return muted; }
            set
            {
                muted = value;
                setMuteImpl();
            }
        }
        private bool muted = false;

        /// <summary>
        /// Audio Position of Playback
        /// </summary>
        public int Position
        {
            get { return position; }
            set
            {
                if (value != position)
                {
                    position = value;
                    PositionChanged?.Invoke();
                }
            }
        }
        private int position = 0;

        public bool Shuffle { get; set; } = false;

        public ArkEchoPlayer()
        {
        }

        public void Start(List<MusicFile> MusicFiles, int Index)
        {
            // TODO: Liste und Position während wiedergabe ändern? -> Playlist starten, dann anders ordnen und trotzdem den nächsten Abspielen
            ListToPlay = MusicFiles;
            SongIndex = Index;

            load(true);
        }

        private void load(bool StartOnLoad)
        {
            disposeImpl();

            loadImpl(StartOnLoad);

            Position = 0;
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
            Position = 0;
            stopImpl();
        }

        public void Forward()
        {
            SongIndex++;
            if (SongIndex == ListToPlay.Count)
            {
                SongIndex = 0;
                load(false);
            }
            else
                load(true);
        }

        //private long lastBackwards = 0;
        public void Backward()
        {
            if (Position > 5 || SongIndex == 0)
            {
                Stop();
                Play();
            }
            else
            {
                SongIndex--;
                load(true);
            }
        }

        public void AudioEnd()
        {
            Forward();
        }

        protected abstract void loadImpl(bool StartOnLoad);
        protected abstract void disposeImpl();
        protected abstract void playImpl();
        protected abstract void pauseImpl();
        protected abstract void playPauseImpl();
        protected abstract void stopImpl();
        protected abstract void setMuteImpl();
        protected abstract void setVolumeImpl();
        protected abstract void setPositionImpl(int NewPosition);
    }
}
