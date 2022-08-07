using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;

namespace ArkEcho.Core.Test
{
    [TestClass]
    public class PlayerTest : MusicTestBase
    {
        private TimerTestPlayer getTestPlayer()
        {
            TimerTestPlayer testPlayer = new();
            return testPlayer;
        }

        [TestMethod]
        public void CreatePlayerTest()
        {
            Player player = getTestPlayer();
            Assert.IsTrue(player.Initialized);
        }

        [TestMethod]
        public void PlayPauseTest()
        {
            Player testPlayer = getTestPlayer();

            Assert.IsTrue(testPlayer.Start(GetTestMusicLibrary().MusicFiles, 0));

            testPlayer.Pause();
            Assert.IsFalse(testPlayer.Playing);

            testPlayer.PlayPause();
            Assert.IsTrue(testPlayer.Playing);

            testPlayer.PlayPause();
            Assert.IsFalse(testPlayer.Playing);
        }

        [TestMethod]
        public void PlayPositionStopTest()
        {
            Player testPlayer = getTestPlayer();

            Assert.IsTrue(testPlayer.Start(GetTestMusicLibrary().MusicFiles, 0));

            Thread.Sleep(1100);

            testPlayer.Stop();

            Assert.IsFalse(testPlayer.Playing);
            Assert.IsTrue(testPlayer.Position == 0);
        }

        [TestMethod]
        public void PositionTest()
        {
            Player testPlayer = getTestPlayer();

            Assert.IsTrue(testPlayer.Start(GetTestMusicLibrary().MusicFiles, 0));

            Thread.Sleep(2100);

            Assert.IsTrue(testPlayer.Playing);
            Assert.IsTrue(testPlayer.Position >= 2);

            testPlayer.Position = 10;

            Thread.Sleep(1100);

            Assert.IsTrue(testPlayer.Position >= 11);
        }
    }
}
