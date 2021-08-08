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

        public bool PlayPause()
        {
            return PlayPauseImpl();
        }

        public bool Stop()
        {
            return StopImpl();
        }

        public bool Forward()
        {
            return ForwardImpl();
        }

        public bool Backward()
        {
            return BackwardImpl();
        }

        public bool AudioFinished()
        {
            // Load next MusicFile
            return true;
        }

        public bool SetMute(bool Mute)
        {
            return SetMuteImpl(Mute);
        }

        protected abstract bool InitImpl(MusicFile File);
        protected abstract bool PlayImpl();
        protected abstract bool PauseImpl();
        protected abstract bool PlayPauseImpl();
        protected abstract bool StopImpl();
        protected abstract bool ForwardImpl();
        protected abstract bool BackwardImpl();
        protected abstract bool SetMuteImpl(bool Mute);
    }
}
