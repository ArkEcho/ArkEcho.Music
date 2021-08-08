using ArkEcho.Core;

namespace ArkEcho.Player
{
    public abstract class ArkEchoPlayer
    {
        public MusicFile PlayingMusic { get; private set; }

        public bool Shuffle { get; set; } = false;

        private int volume = 50;
        public int Volume { get { return volume; } set { volume = value; SetVolumeImpl(value); } }

        private bool muted = false;
        public bool Mute { get { return muted; } set { muted = value; SetMuteImpl(value); } }

        public ArkEchoPlayer()
        {
        }

        public void Init(MusicFile File)
        {
            DisposeImpl();

            this.PlayingMusic = File;
            InitImpl(File);

            SetMuteImpl(muted);
            SetVolumeImpl(volume);
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
            // TODO: Set to 0, if double klicked backward
        }

        public void AudioEnd()
        {
            // TODO: Load next MusicFile
        }

        protected abstract void InitImpl(MusicFile File);
        protected abstract void DisposeImpl();
        protected abstract void PlayImpl();
        protected abstract void PauseImpl();
        protected abstract void PlayPauseImpl();
        protected abstract void StopImpl();
        protected abstract void SetMuteImpl(bool Mute);
        protected abstract void SetVolumeImpl(int NewVolume);
    }
}
