using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Threading;

namespace ArkEcho.Core.Test
{
    [TestClass]
    public class PlayerTest : MusicTestBase
    {
        private void CreatePlayer(out Player player)
        {
            player = new TimerTestPlayer();
            Assert.IsTrue(player.Initialized);
        }

        private void StartPlayer(out Player testPlayer, out List<MusicFile> fileList)
        {
            CreatePlayer(out testPlayer);
            fileList = GetTestMusicLibrary().MusicFiles;

            bool started = testPlayer.Start(fileList, 0);
            Assert.IsTrue(started);
        }

        [TestMethod]
        public void PlayPause()
        {
            StartPlayer(out Player testPlayer, out List<MusicFile> fileList);

            testPlayer.Pause();
            Assert.IsFalse(testPlayer.Playing);

            testPlayer.Play();
            Assert.IsTrue(testPlayer.Playing);

            testPlayer.PlayPause();
            Assert.IsFalse(testPlayer.Playing);

            testPlayer.PlayPause();
            Assert.IsTrue(testPlayer.Playing);
        }

        [TestMethod]
        public void Stop()
        {
            StartPlayer(out Player testPlayer, out List<MusicFile> fileList);

            Thread.Sleep(200);

            testPlayer.Stop();

            Assert.IsFalse(testPlayer.Playing);
            Assert.IsTrue(testPlayer.Position == 0);
        }

        [TestMethod]
        public void Position()
        {
            StartPlayer(out Player testPlayer, out List<MusicFile> fileList);

            Thread.Sleep(1100);

            Assert.IsTrue(testPlayer.Playing);
            Assert.IsTrue(testPlayer.Position >= 1);

            testPlayer.Position = 10;

            Thread.Sleep(1100);

            Assert.IsTrue(testPlayer.Position >= 11);
        }

        [TestMethod]
        public void AutoLoadNextSong()
        {
            StartPlayer(out Player testPlayer, out List<MusicFile> fileList);

            Assert.IsTrue(testPlayer.PlayingFile.Track == 1);

            testPlayer.Position = 19;

            Thread.Sleep(1500);

            Assert.IsTrue(testPlayer.PlayingFile.Track == 2);
        }

        [TestMethod]
        public void ForwardBackward()
        {
            StartPlayer(out Player testPlayer, out List<MusicFile> fileList);

            testPlayer.Forward();
            testPlayer.Forward();
            testPlayer.Forward();

            testPlayer.Backward();
            testPlayer.Backward();

            Assert.IsTrue(testPlayer.PlayingFile.Track == 2);
        }

        [TestMethod]
        public void BackwardRestartSongOverFive()
        {
            StartPlayer(out Player testPlayer, out List<MusicFile> fileList);

            for (int i = 0; i < 3; i++)
                testPlayer.Forward();

            testPlayer.Position = 7;
            Thread.Sleep(500);

            testPlayer.Backward();
            Thread.Sleep(500);

            Assert.IsTrue(testPlayer.Playing);
            Assert.IsTrue(testPlayer.Position <= 1);
            Assert.IsTrue(testPlayer.PlayingFile.Track == 4);
        }

        [TestMethod]
        public void LoadFirstSongStoppedOnEndOfList()
        {
            StartPlayer(out Player testPlayer, out List<MusicFile> fileList);

            for (int i = 0; i < fileList.Count - 1; i++)
                testPlayer.Forward();

            Assert.IsTrue(testPlayer.PlayingFile.Track == fileList.Last().Track);

            testPlayer.Position = (fileList.Last().Duration / 1000) - 1;
            Thread.Sleep(2000);

            Assert.IsFalse(testPlayer.Playing);
            Assert.IsTrue(testPlayer.Position == 0);
            Assert.IsTrue(testPlayer.PlayingFile.GUID == fileList[0].GUID);
        }
    }
}
