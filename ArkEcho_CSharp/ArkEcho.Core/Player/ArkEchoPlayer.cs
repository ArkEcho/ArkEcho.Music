using ArkEcho.Core;

namespace ArkEcho.Player
{
    public abstract class ArkEchoPlayer
    {
        public MusicFile PlayingMusic { get; private set; }

        public bool Shuffle { get; set; } = false;

        public ArkEchoPlayer()
        {
        }

        public bool Init(MusicFile File)
        {
            this.PlayingMusic = File;
            return InitImpl(File);
        }

        public bool Play()
        {
            return PlayImpl();
        }

        public bool Pause()
        {
            return PauseImpl();
        }

        public bool Stop()
        {
            return StopImpl();
        }

        public bool AudioFinished()
        {
            // Load next MusicFile
            return true;
        }

        protected abstract bool InitImpl(MusicFile File);
        protected abstract bool PlayImpl();
        protected abstract bool PauseImpl();
        protected abstract bool StopImpl();
    }
}
