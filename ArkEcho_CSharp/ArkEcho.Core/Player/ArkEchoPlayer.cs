using ArkEcho.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ArkEcho.Player
{
    // TODO: JSONBase -> Einstellungen nach Nutzer speichern
    public abstract class ArkEchoPlayer
    {
        public event Action TitleChanged;
        public event Action PositionChanged;
        public event Action PlayingChanged;

        public List<MusicFile> ListToPlay { get; private set; } = null;

        private int songIndex = 0;

        public MusicFile GetPlayingFile()
        {
            return ListToPlay != null ? ListToPlay.Count > songIndex && songIndex >= 0 ? ListToPlay[songIndex] : null : null;
        }

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
        /// Audio Muted
        /// </summary>
        public bool Playing
        {
            get { return playing; }
            protected set
            {
                playing = value;
                PlayingChanged?.Invoke();
            }
        }
        private bool playing = false;

        /// <summary>
        /// Audio Position of Playback
        /// </summary>
        public int Position
        {
            get { return position; }
            protected set
            {
                if (value != position)
                {
                    position = value;
                    PositionChanged?.Invoke();
                }
            }
        }
        private int position = 0;

        // TODO: Shuffle
        public bool Shuffle { get; set; } = false;
        private List<int> shuffledIndexList = null;

        public bool Initialized { get; protected set; }

        public ArkEchoPlayer() { }

        public void Start(List<MusicFile> MusicFiles, int Index)
        {
            logImpl($"Start {MusicFiles.Count} Files", Resources.LogLevel.Information);

            // TODO: Liste und Position während wiedergabe ändern? -> Playlist starten, dann anders ordnen und trotzdem den nächsten Abspielen
            ListToPlay = MusicFiles;
            songIndex = Index;

            shuffledIndexList = RandomShuffle.GetShuffledList(Enumerable.Range(0, ListToPlay.Count - 1).ToList());

            load(true);
        }

        private void load(bool StartOnLoad)
        {
            disposeImpl();
            Position = 0;

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
            if (playing)
                Pause();
            else
                Play();
        }

        public void Stop()
        {
            stopImpl();
            Position = 0;
        }

        public void Forward()
        {
            songIndex++;
            if (songIndex == ListToPlay.Count)
            {
                songIndex = 0;
                load(false);
            }
            else
                load(true);
        }

        //private long lastBackwards = 0;
        public void Backward()
        {
            if (Position > 5 || songIndex == 0)
            {
                Stop();
                Play();
            }
            else
            {
                songIndex--;
                load(true);
            }
        }

        public void SetPosition(int NewPosition)
        {
            setPositionImpl(NewPosition);
        }

        public void AudioEnd()
        {
            Forward();
        }

        protected abstract bool logImpl(string Text, Resources.LogLevel Level);
        protected abstract void loadImpl(bool StartOnLoad);
        protected abstract void disposeImpl();
        protected abstract void playImpl();
        protected abstract void pauseImpl();
        protected abstract void stopImpl();
        protected abstract void setMuteImpl();
        protected abstract void setVolumeImpl();
        protected abstract void setPositionImpl(int NewPosition);
    }
}
