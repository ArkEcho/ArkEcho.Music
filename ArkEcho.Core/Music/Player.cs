using System;
using System.Collections.Generic;
using System.Linq;

namespace ArkEcho.Core
{
    // TODO: Mehr Logging
    // TODO: Position in 0-1 decimal?
    // TODO: Repeat Playlist Funktion
    // TODO: Liste und Position während wiedergabe ändern? -> Playlist starten, dann anders ordnen und trotzdem den nächsten Abspielen

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
                if (value < 0 || value > 100)
                    return;

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
            get
            {
                return position;
            }
            set
            {
                if (value == position || value < 0)
                    return;

                // Pause and give Player time to react
                bool playing = Playing;
                if (playing)
                    Pause();

                position = value;
                setAudioPosition();

                if (playing)
                    Play();
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
                setShuffleList(Guid.Empty, -1);

                if (shuffle || listToPlay.IsNullOrEmpty() || PlayingFile == null)
                    return;

                if (listToPlay.IndexOf(PlayingFile) != songIndex) // restore unshuffled SongIndex
                    songIndex = shuffledIndexList[songIndex];
            }
        }
        private bool shuffle = false;

        private List<int> shuffledIndexList = null;

        private void setShuffleList(Guid lastPlayingGuid, int startingIndex)
        {
            void createShuffledIndexList()
            {
                shuffledIndexList = RandomShuffle.GetShuffledList(Enumerable.Range(0, listToPlay.Count).ToList());
            }

            if (!shuffle || listToPlay.IsNullOrEmpty())
                return;

            if (listToPlay.Count == 1)
                shuffledIndexList = new List<int>() { 0 };
            else if (startingIndex >= 0) // Given by Start(), to begin the shuffled List with the given IndexToStart
            {
                createShuffledIndexList();
                shuffledIndexList.Remove(startingIndex);
                shuffledIndexList.Insert(0, startingIndex);
            }
            else if (lastPlayingGuid != Guid.Empty) // Given by caller Forward/BackWard, last played Guid must not be next song
            {
                do
                    createShuffledIndexList();
                while (listToPlay[shuffledIndexList[songIndex]].GUID == lastPlayingGuid);
            }
            else // Shuffle was turned off/on externally while Started
            {
                if (PlayingFile == null)
                    return;

                // Create new, if this AND next Song is the same as playing
                Guid playingGuid = PlayingFile.GUID;
                if (listToPlay.Count == 2)
                {
                    do
                        createShuffledIndexList();
                    while (listToPlay[shuffledIndexList[songIndex]].GUID != playingGuid); // Playing song must be actual Song
                }
                else
                {
                    do
                        createShuffledIndexList();
                    while (listToPlay[shuffledIndexList[songIndex]].GUID == playingGuid ||
                    (songIndex + 1 < shuffledIndexList.Count && listToPlay[shuffledIndexList[songIndex + 1]].GUID == playingGuid));
                }
            }
        }

        #endregion

        public bool Start(List<MusicFile> listToPlay, int index)
        {
            if (listToPlay == null || index < 0 || index >= listToPlay.Count)
                return false;

            this.listToPlay = listToPlay;

            log($"Starting {listToPlay.Count} Files", Logging.LogLevel.Important);

            if (shuffle)
                setShuffleList(Guid.Empty, index);
            else
                songIndex = index;

            loadNextPlayingFile(true);
            return Playing;
        }

        private void loadNextPlayingFile(bool StartOnLoad)
        {
            disposeAudio();
            position = 0;

            PlayingFile = Shuffle ? listToPlay[shuffledIndexList[songIndex]] : listToPlay[songIndex];

            if (PlayingFile == null)
                return;

            loadAudio(StartOnLoad);
            TitleChanged?.Invoke();

            if (!StartOnLoad)
                playingChanged(false);
        }

        public void Play()
        {
            if (PlayingFile == null)
                return;

            playAudio();
        }

        public void Pause()
        {
            if (PlayingFile == null)
                return;

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
            if (PlayingFile == null)
                return;

            stopAudio();
            position = 0;
        }

        public void Forward()
        {
            if (listToPlay.IsNullOrEmpty())
                return;

            if (songIndex + 1 == listToPlay.Count)
            {
                Guid lastGuid = PlayingFile.GUID;
                songIndex = 0;

                if (Shuffle) // Reset shuffle Order and start new Song
                {
                    setShuffleList(lastGuid, -1);
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
            if (listToPlay.IsNullOrEmpty())
                return;

            if (position > 5)
            {
                Stop();
                Play();
            }
            else if (songIndex == 0)
            {
                if (Shuffle)
                {
                    Guid lastGuid = listToPlay[shuffledIndexList[songIndex]].GUID;
                    setShuffleList(lastGuid, -1);
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
