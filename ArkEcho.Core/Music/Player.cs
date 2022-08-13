using System;
using System.Collections.Generic;
using System.Linq;

namespace ArkEcho.Core
{
    // TODO: Unit Test
    // TODO: Aktivieren von Shuffle bei nur 1 Song = Exception
    // TODO: Mehr Logging
    // TODO: Umstellung auf Interface von Implementierungen?
    // TODO: Position in 0-1 decimal?
    public abstract class Player
    {
        public event Action TitleChanged;
        public event Action PositionChanged;
        public event Action PlayingChanged;

        public Player() { }

        public bool Initialized { get; protected set; }

        private List<MusicFile> listToPlay = null;

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

        public bool Playing
        {
            get { return playing; }
        }
        private bool playing = false;

        protected void playingChanged(bool playing)
        {
            this.playing = playing;
            PlayingChanged?.Invoke();
        }

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
                    setAudioPosition();
                }
            }
        }
        private int position = 0;

        protected void audioPositionChanged(int position)
        {
            this.position = position;
            PositionChanged?.Invoke();
        }

        #endregion

        #region Shuffle

        public bool Shuffle
        {
            get { return shuffle; }
            set
            {
                shuffle = value;
                setShuffleList();
                if (!shuffle && listToPlay != null)
                {
                    int indexPlaying = listToPlay.IndexOf(PlayingFile);
                    if (indexPlaying >= 0 && indexPlaying != songIndex)
                        songIndex = shuffledIndexList[songIndex];
                }
            }
        }
        private bool shuffle = false;

        private List<int> shuffledIndexList = null;

        private void setShuffleList()
        {
            if (shuffle && listToPlay != null)
            {
                shuffledIndexList = RandomShuffle.GetShuffledList(Enumerable.Range(0, listToPlay.Count - 1).ToList());
                if (songIndex == shuffledIndexList[songIndex] || songIndex == (shuffledIndexList[songIndex + 1]))
                    setShuffleList();
            }
        }

        #endregion

        public bool Start(List<MusicFile> MusicFiles, int Index)
        {
            if (MusicFiles == null || Index < 0 || Index >= MusicFiles.Count)
                return false;

            // TODO: Liste und Position während wiedergabe ändern? -> Playlist starten, dann anders ordnen und trotzdem den nächsten Abspielen
            listToPlay = MusicFiles;
            songIndex = Index;

            log($"Starting {MusicFiles.Count} Files", Logging.LogLevel.Important);

            setShuffleList();

            loadNextPlayingFile(true);
            return Playing;
        }

        private void loadNextPlayingFile(bool StartOnLoad)
        {
            disposeAudio();
            Position = 0;

            PlayingFile = Shuffle ? listToPlay[shuffledIndexList[songIndex]] : listToPlay[songIndex];

            if (PlayingFile == null)
                return;

            loadAudio(StartOnLoad);
            TitleChanged?.Invoke();
        }

        public void Play()
        {
            if (PlayingFile == null)
                return;

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
            if (listToPlay == null)
                return;

            if (songIndex + 1 == listToPlay.Count)
            {
                songIndex = 0;
                if (Shuffle) // Reset shuffle Order and start new Song
                {
                    setShuffleList();
                    loadNextPlayingFile(true);
                }
                else // Reached end of List, load first but dont start
                    loadNextPlayingFile(false);
            }
            else
            {
                songIndex++;
                loadNextPlayingFile(true);
            }
        }

        public void Backward()
        {
            if (listToPlay == null)
                return;

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
