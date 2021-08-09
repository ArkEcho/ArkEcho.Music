using ArkEcho.Core;

namespace ArkEcho.Player
{
    public abstract class ArkEchoPlayer
    {
        public MusicFile PlayingMusic { get; private set; }

        public bool Shuffle { get; set; } = false;

        /// <summary>
        /// Volume, 0 - 100
        /// </summary>
        public int Volume { get { return volume; } set { volume = value; SetVolumeImpl(); } }
        private int volume = 50;

        public bool Mute { get { return muted; } set { muted = value; SetMuteImpl(); } }
        private bool muted = false;

        /// <summary>
        /// Set true to start the Audio on Init()
        /// </summary>
        public bool DirectPlay { get; set; } = false;

        public ArkEchoPlayer()
        {
        }

        public void Init(MusicFile File)
        {
            this.PlayingMusic = File;

            DisposeImpl();

            InitImpl();
        }

        public void Play()
        {
            PlayImpl();
        }

        public void Pause()
        {
            PauseImpl();
        }

        public void PlayPause()
        {
            PlayPauseImpl();
        }

        public void Stop()
        {
            StopImpl();
        }

        public void Forward()
        {
            // TODO: Load next MusicFile
        }

        public void Backward()
        {
            // TODO: Set to 0 (Stop and Play), if double klicked backward
        }

        public void AudioEnd()
        {
            // TODO: Load next MusicFile
        }

        protected abstract void InitImpl();
        protected abstract void DisposeImpl();
        protected abstract void PlayImpl();
        protected abstract void PauseImpl();
        protected abstract void PlayPauseImpl();
        protected abstract void StopImpl();
        protected abstract void SetMuteImpl();
        protected abstract void SetVolumeImpl();
    }
}
