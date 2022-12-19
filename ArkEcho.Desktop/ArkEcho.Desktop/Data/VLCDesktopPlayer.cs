using ArkEcho.Core;

namespace ArkEcho.Desktop
{
    public class VLCDesktopPlayer : Player
    {
        public VLCDesktopPlayer() : base()
        {
        }

        protected override void disposeAudio()
        {
            throw new NotImplementedException();
        }

        protected override void loadAudio(bool StartOnLoad)
        {
            throw new NotImplementedException();
        }

        protected override bool log(string Text, Logging.LogLevel Level)
        {
            throw new NotImplementedException();
        }

        protected override void pauseAudio()
        {
            throw new NotImplementedException();
        }

        protected override void playAudio()
        {
            throw new NotImplementedException();
        }

        protected override void setAudioMute()
        {
            throw new NotImplementedException();
        }

        protected override void setAudioPosition()
        {
            throw new NotImplementedException();
        }

        protected override void setAudioVolume()
        {
            throw new NotImplementedException();
        }

        protected override void stopAudio()
        {
            throw new NotImplementedException();
        }
    }
}
