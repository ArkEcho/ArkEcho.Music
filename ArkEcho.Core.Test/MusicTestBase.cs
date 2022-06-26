using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ArkEcho.Core.Test
{
    [TestClass]
    public class MusicTestBase
    {
        public MusicLibrary GetTestMusicLibrary()
        {
            MusicLibrary library = new MusicLibrary();

            library.MusicFiles.Add(new MusicFile() { Track = 1, Duration = 2500 });
            library.MusicFiles.Add(new MusicFile() { Track = 2, Duration = 3000 });
            library.MusicFiles.Add(new MusicFile() { Track = 3, Duration = 3500 });
            library.MusicFiles.Add(new MusicFile() { Track = 4, Duration = 1000 });
            library.MusicFiles.Add(new MusicFile() { Track = 5, Duration = 2000 });
            library.MusicFiles.Add(new MusicFile() { Track = 6, Duration = 4000 });
            library.MusicFiles.Add(new MusicFile() { Track = 7, Duration = 5000 });

            return library;
        }

        public class TestPlayer : Player
        {
            public TestPlayer()
            {
                Initialized = true;
            }

            protected override void disposeImpl()
            {
            }

            protected override void loadImpl(bool StartOnLoad)
            {
            }

            protected override bool logImpl(string Text, Logging.LogLevel Level)
            {
                return true;
            }

            protected override void pauseImpl()
            {
            }

            protected override void playImpl()
            {
            }

            protected override void setMuteImpl()
            {
            }

            protected override void setPositionImpl(int NewPosition)
            {
            }

            protected override void setVolumeImpl()
            {
            }

            protected override void stopImpl()
            {
            }
        }
    }
}
