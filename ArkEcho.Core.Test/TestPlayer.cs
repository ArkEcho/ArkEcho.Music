using System.Timers;

namespace ArkEcho.Core.Test
{
    public partial class MusicTestBase
    {
        public class TestPlayer : Player
        {
            private Timer timer = null;
            private int positionSeconds = 0;
            private long interval = 1000;

            public TestPlayer()
            {
                Initialized = true;
                timer = new Timer()
                {
                    Interval = interval,
                    Enabled = true,
                };
                timer.Elapsed += Timer_Elapsed;
            }

            private void Timer_Elapsed(object sender, ElapsedEventArgs e)
            {
                positionSeconds++;
                Position = positionSeconds;

                if (positionSeconds >= PlayingFile?.Duration / 1000)
                    AudioEnd();
                else
                    timer.Start();
            }

            protected override void disposeAudio()
            {
                timer.Stop();
            }

            protected override void loadAudio(bool StartOnLoad)
            {
                if (StartOnLoad)
                    timer.Start();
            }

            protected override bool log(string Text, Logging.LogLevel Level)
            {
                return true;
            }

            protected override void pauseAudio()
            {
                timer.Stop();
            }

            protected override void playAudio()
            {
                timer.Start();
            }

            protected override void setAudioMute()
            {
            }

            protected override void setAudioPosition()
            {
                if (Position != positionSeconds)
                    positionSeconds = Position;
            }

            protected override void setAudioVolume()
            {
            }

            protected override void stopAudio()
            {
                timer.Stop();
            }
        }
    }
}
