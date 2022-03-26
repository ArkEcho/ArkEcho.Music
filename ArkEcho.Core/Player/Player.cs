using System;
using System.Collections.Generic;
using System.Linq;

namespace ArkEcho.Core
{
    // TODO: JSONBase -> Einstellungen nach Nutzer speichern
    // TODO: Aktivieren von Shuffle bei nur 1 Song = Exception
    public abstract class Player
    {
        public event Action TitleChanged;
        public event Action PositionChanged;
        public event Action PlayingChanged;

        public Player() { }

        public bool Initialized { get; protected set; }

        public List<MusicFile> ListToPlay { get; private set; } = null;

        public MusicFile PlayingFile { get; private set; } = null;

        private int songIndex = 0;

        private void setPlayingFile()
        {
            if (ListToPlay != null && ListToPlay.Count > songIndex && songIndex >= 0)
                PlayingFile = Shuffle ? ListToPlay[shuffledIndexList[songIndex]] : ListToPlay[songIndex];
            else
                PlayingFile = null;
        }


        #region Volume

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

        #endregion

        #region Mute

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

        #endregion

        #region Playing

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

        #endregion

        #region Position

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

        #endregion

        #region Shuffle

        public bool Shuffle
        {
            get { return shuffle; }
            set
            {
                shuffle = value;
                setShuffleList();
                if (!shuffle && ListToPlay != null)
                {
                    int indexPlaying = ListToPlay.IndexOf(PlayingFile);
                    if (indexPlaying >= 0 && indexPlaying != songIndex)
                        songIndex = shuffledIndexList[songIndex];
                }
            }
        }
        private bool shuffle = false;

        private List<int> shuffledIndexList = null;

        private void setShuffleList()
        {
            if (shuffle && ListToPlay != null)
            {
                shuffledIndexList = RandomShuffle.GetShuffledList(Enumerable.Range(0, ListToPlay.Count - 1).ToList());
                if (songIndex == shuffledIndexList[songIndex] || songIndex == (shuffledIndexList[songIndex + 1]))
                    setShuffleList();
            }
        }

        #endregion

        public void Start(List<MusicFile> MusicFiles, int Index)
        {
            logImpl($"Starting {MusicFiles.Count} Files", Logging.LogLevel.Important);

            // TODO: Liste und Position während wiedergabe ändern? -> Playlist starten, dann anders ordnen und trotzdem den nächsten Abspielen
            ListToPlay = MusicFiles;
            songIndex = Index;

            setShuffleList();

            load(true);
        }

        private void load(bool StartOnLoad)
        {
            disposeImpl();
            Position = 0;

            setPlayingFile();

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
            if (ListToPlay != null)
            {
                if (songIndex + 1 == ListToPlay.Count)
                {
                    songIndex = 0;
                    if (Shuffle)
                    {
                        setShuffleList();
                        load(true);
                    }
                    else
                        load(false);
                }
                else
                {
                    songIndex++;
                    load(true);
                }
            }
        }

        public void Backward()
        {
            if (ListToPlay != null)
            {
                if (Position > 5)
                {
                    Stop();
                    Play();
                }
                else if (songIndex == 0)
                {
                    if (Shuffle)
                    {
                        setShuffleList();
                        load(true);
                    }
                    else
                    {
                        Stop();
                        Play();
                    }
                }
                else
                {
                    songIndex--;
                    load(true);
                }
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

        protected abstract bool logImpl(string Text, Logging.LogLevel Level);
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
