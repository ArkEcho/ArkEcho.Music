using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Threading;

namespace ArkEcho.Core.Test
{
    // TODO: Weniger feste Nummern -> mehr Random aus der Liste, zufällig grosse Liste etc.
    [TestClass]
    public class PlayerTest : MusicTestBase
    {
        private void getPlayer(out Player testPlayer)
        {
            testPlayer = new TimerTestPlayer();
        }

        private void getFileList(int count, out List<MusicFile> fileList)
        {
            fileList = new List<MusicFile>();

            for (int i = 0; i < count; i++)
                fileList.Add(GetTestMusicLibrary().MusicFiles[i]);
        }

        [TestMethod]
        public void EmptyPlayerAndFalseValues()
        {
            Player player = new TimerTestPlayer();

            player.Forward();
            player.Backward();
            player.Pause();
            player.Play();
            player.PlayPause();
            player.Stop();

            player.Volume = 5;
            player.Volume = 500;
            Assert.AreEqual(player.Volume, 5);

            player.Volume = -20;
            Assert.AreEqual(player.Volume, 5);

            player.Shuffle = true;
            player.Shuffle = false;

            player.Position = 5;
            player.Position = -20;
            Assert.AreEqual(player.Position, 5);

            player.Mute = true;
            player.Mute = false;

            bool started = player.Start(new List<MusicFile>(), -1);
            Assert.IsFalse(started);

            started = player.Start(null, 0);
            Assert.IsFalse(started);
        }

        [TestMethod]
        public void PlayPause()
        {
            getPlayer(out Player testPlayer);
            getFileList(7, out List<MusicFile> files);

            testPlayer.Start(files, 0);

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
            getPlayer(out Player testPlayer);
            getFileList(7, out List<MusicFile> files);

            testPlayer.Start(files, 0);

            Thread.Sleep(200);

            testPlayer.Stop();

            Assert.IsFalse(testPlayer.Playing);
            Assert.IsTrue(testPlayer.Position == 0);
        }

        [TestMethod]
        public void Position()
        {
            getPlayer(out Player testPlayer);
            getFileList(7, out List<MusicFile> files);

            testPlayer.Start(files, 0);

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
            getPlayer(out Player testPlayer);
            getFileList(7, out List<MusicFile> files);

            testPlayer.Start(files, 0);

            Assert.IsTrue(testPlayer.PlayingFile.Track == 1);

            testPlayer.Position = 19;

            Thread.Sleep(2000);

            Assert.IsTrue(testPlayer.PlayingFile.Track == 2);
        }

        [TestMethod]
        public void ForwardBackward()
        {
            getPlayer(out Player testPlayer);
            getFileList(7, out List<MusicFile> files);

            testPlayer.Start(files, 0);

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
            getPlayer(out Player testPlayer);
            getFileList(7, out List<MusicFile> files);

            testPlayer.Start(files, 0);

            for (int i = 0; i < 3; i++)
                testPlayer.Forward();

            testPlayer.Position = 7;
            Thread.Sleep(500);

            testPlayer.Backward();
            Thread.Sleep(500);

            testPlayer.Playing.Should().BeTrue();
            testPlayer.Position.Should().BeLessThanOrEqualTo(2);
            Assert.IsTrue(testPlayer.PlayingFile.Track == 4);
        }

        [TestMethod]
        public void LoadFirstSongStoppedOnEndOfListWhilePlaying()
        {
            getPlayer(out Player testPlayer);
            getFileList(7, out List<MusicFile> files);

            testPlayer.Start(files, 0);

            for (int i = 0; i < files.Count - 1; i++)
                testPlayer.Forward();

            Assert.IsTrue(testPlayer.PlayingFile.Track == files.Last().Track);

            testPlayer.Position = (files.Last().Duration / 1000) - 1;
            Thread.Sleep(2000);

            Assert.IsTrue(!testPlayer.Playing);
            Assert.IsTrue(testPlayer.Position == 0);
            Assert.IsTrue(testPlayer.PlayingFile.GUID == files[0].GUID);
        }

        [TestMethod]
        public void LoadSongStoppedOnEndOfListWithForward()
        {
            getPlayer(out Player testPlayer);
            getFileList(1, out List<MusicFile> files);

            testPlayer.Start(files, 0);

            testPlayer.Forward();

            Assert.IsTrue(!testPlayer.Playing);
            Assert.IsTrue(testPlayer.Position == 0);
            Assert.IsTrue(testPlayer.PlayingFile.GUID == files[0].GUID);
        }

        [TestMethod]
        public void ShuffleOneSong()
        {
            getPlayer(out Player testPlayer);

            getFileList(1, out List<MusicFile> files);

            testPlayer.Shuffle = true;

            bool started = testPlayer.Start(files, 0);

            Assert.IsTrue(started);
            Assert.IsTrue(testPlayer.Playing);

            testPlayer.Forward();
            Assert.IsTrue(testPlayer.Playing);

            testPlayer.Position = 19;
            Thread.Sleep(1500);
            Assert.IsTrue(testPlayer.Playing);

            testPlayer.Position = 6;
            testPlayer.Backward();
            Assert.IsTrue(testPlayer.Playing);
        }

        [TestMethod]
        public void ShuffleTwoSongs()
        {
            getPlayer(out Player testPlayer);

            getFileList(2, out List<MusicFile> files);

            testPlayer.Shuffle = true;

            // Start Test
            bool started = testPlayer.Start(files, 0);

            Assert.IsTrue(started);
            Assert.IsTrue(testPlayer.Playing);

            // Let Forward start over
            testPlayer.Forward();
            testPlayer.Forward();

            Assert.IsTrue(testPlayer.Playing);

            // Turn Shuttle on
            testPlayer.Shuffle = true;
            Assert.IsTrue(testPlayer.Playing);

            // Check, that both Songs get played alternating
            int trackOnePlayed = 0;
            for (int i = 0; i < 6; i++)
            {
                if (testPlayer.PlayingFile.Track == 1) trackOnePlayed++;
                testPlayer.Forward();
            }
            Assert.IsTrue(trackOnePlayed == 3);
        }

        [TestMethod]
        public void ShuffleStartsWithGivenIndex()
        {
            getPlayer(out Player testPlayer);
            getFileList(7, out List<MusicFile> files);

            testPlayer.Shuffle = true;
            testPlayer.Start(files, 0);

            for (int i = 0; i <= files.Count - 1; i++)
            {
                bool started = testPlayer.Start(files, i);

                Assert.IsTrue(started);
                Assert.IsTrue(files[i].Track == testPlayer.PlayingFile.Track);
            }
        }

        [TestMethod]
        public void ShuffleEverySongOnlyOnceAndRestart()
        {
            getPlayer(out Player testPlayer);
            getFileList(7, out List<MusicFile> files);

            testPlayer.Shuffle = true;

            testPlayer.Start(files, 0);

            testPlayer.Backward();

            List<int> playedTracks = new List<int>();

            for (int i = 0; i <= files.Count - 1; i++)
            {
                playedTracks.Add(testPlayer.PlayingFile.Track);
                testPlayer.Forward();
            }

            for (int i = 0; i <= files.Count - 1; i++)
            {
                playedTracks.Remove(testPlayer.PlayingFile.Track);
                testPlayer.Forward();
            }

            Assert.IsTrue(playedTracks.Count == 0);
            Assert.IsTrue(testPlayer.Playing);
        }

        [TestMethod]
        public void ShuffleNextSongIsDifferent()
        {
            getPlayer(out Player testPlayer);
            getFileList(7, out List<MusicFile> files);

            testPlayer.Start(files, 0);

            for (int i = 0; i <= 10000; i++)
            {
                testPlayer.Shuffle = true;

                int track = testPlayer.PlayingFile.Track;
                testPlayer.Forward();

                Assert.IsFalse(track == testPlayer.PlayingFile.Track, $"Next Song on Shuffle is the Same!");
            }
        }

        [TestMethod]
        public void SwitchShuffleOffOnAndContinueRightOrder()
        {
            getPlayer(out Player testPlayer);
            getFileList(7, out List<MusicFile> files);

            testPlayer.Start(files, 0);

            bool flipOverOff = false;
            bool justNextSongOff = false;

            while (flipOverOff == false || justNextSongOff == false)
            {
                testPlayer.Shuffle = true;

                testPlayer.Forward();
                testPlayer.Forward();

                int track = testPlayer.PlayingFile.Track;
                testPlayer.Shuffle = false;
                testPlayer.Forward();

                if (files.Last().Track == track) // Last Track loaded, shuffle off and Forward = first Track
                {
                    Assert.IsTrue(testPlayer.PlayingFile.Track == 1);
                    flipOverOff = true;
                }
                else
                {
                    Assert.IsTrue(track + 1 == testPlayer.PlayingFile.Track);
                    justNextSongOff = true;
                }
            }
        }
    }
}
