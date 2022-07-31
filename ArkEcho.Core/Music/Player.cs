using System;
using System.Collections.Generic;
using System.Linq;

namespace ArkEcho.Core
{
    // TODO: Unit Test
    // TODO: Aktivieren von Shuffle bei nur 1 Song = Exception
    // TODO: Mehr Logging
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
                setAudioVolume();
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
                setAudioMute();
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
        /// Audio Position of Playback in Seconds
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
                    setAudioPosition();
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
            log($"Starting {MusicFiles.Count} Files", Logging.LogLevel.Important);

            // TODO: Liste und Position während wiedergabe ändern? -> Playlist starten, dann anders ordnen und trotzdem den nächsten Abspielen
            ListToPlay = MusicFiles;
            songIndex = Index;

            setShuffleList();

            loadNextPlayingFile(true);
        }

        private void loadNextPlayingFile(bool StartOnLoad)
        {
            disposeAudio();
            Position = 0;

            PlayingFile = null;

            if (ListToPlay == null || songIndex >= ListToPlay.Count || songIndex < 0)
                return;

            PlayingFile = Shuffle ? ListToPlay[shuffledIndexList[songIndex]] : ListToPlay[songIndex];

            if (PlayingFile != null)
            {
                loadAudio(StartOnLoad);
                TitleChanged?.Invoke();
            }
        }

        public void Play()
        {
            playAudio();
        }

        public void Pause()
        {
            pauseAudio();
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
            stopAudio();
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
                        loadNextPlayingFile(true);
                    }
                    else
                        loadNextPlayingFile(false);
                }
                else
                {
                    songIndex++;
                    loadNextPlayingFile(true);
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
                        loadNextPlayingFile(true);
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
                    loadNextPlayingFile(true);
                }
            }
        }

        protected void AudioEnd()
        {
            Forward();
        }

        protected abstract bool log(string Text, Logging.LogLevel Level);
        protected abstract void loadAudio(bool StartOnLoad);
        protected abstract void disposeAudio();
        protected abstract void playAudio();
        protected abstract void pauseAudio();
        protected abstract void stopAudio();
        protected abstract void setAudioMute();
        protected abstract void setAudioVolume();
        protected abstract void setAudioPosition();
    }
}
