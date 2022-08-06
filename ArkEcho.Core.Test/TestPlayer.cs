using System;
using System.Timers;

namespace ArkEcho.Core.Test
{
    public partial class MusicTestBase
    {
        public class TestPlayer : Player
        {
            private Timer timer = null;
            private int positionTenthSeconds = 0;
            private long interval = 100;

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
                positionTenthSeconds++;
                audioPositionChanged(getPositionInSeconds());

                if (positionTenthSeconds >= PlayingFile?.Duration / 100)
                {
                    playingChanged(false);
                    AudioEnd();
                }
                else
                    timer.Start();
            }

            private int getPositionInSeconds()
            {
                double tes = positionTenthSeconds / 10;
                return Convert.ToInt32(Math.Round(tes));
            }

            protected override void disposeAudio()
            {
                playingChanged(false);
                timer.Stop();
            }

            protected override void loadAudio(bool StartOnLoad)
            {
                if (StartOnLoad)
                {
                    timer.Start();
                    playingChanged(true);
                }
            }

            protected override bool log(string Text, Logging.LogLevel Level)
            {
                return true;
            }

            protected override void pauseAudio()
            {
                timer.Stop();
                playingChanged(false);
            }

            protected override void playAudio()
            {
                timer.Start();
                playingChanged(true);
            }

            protected override void setAudioMute()
            {
            }

            protected override void setAudioPosition()
            {
                positionTenthSeconds = Position * 10;
            }

            protected override void setAudioVolume()
            {
            }

            protected override void stopAudio()
            {
                timer.Stop();
                playingChanged(false);
            }
        }
    }
}
