using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ArkEcho.Core.Test
{
    [TestClass]
    public partial class MusicTestBase
    {
        public MusicLibrary GetTestMusicLibrary()
        {
            MusicLibrary library = new MusicLibrary();

            library.MusicFiles.Add(new MusicFile() { Track = 1, Duration = 20000 });
            library.MusicFiles.Add(new MusicFile() { Track = 2, Duration = 3000 });
            library.MusicFiles.Add(new MusicFile() { Track = 3, Duration = 3500 });
            library.MusicFiles.Add(new MusicFile() { Track = 4, Duration = 1000 });
            library.MusicFiles.Add(new MusicFile() { Track = 5, Duration = 2000 });
            library.MusicFiles.Add(new MusicFile() { Track = 6, Duration = 4000 });
            library.MusicFiles.Add(new MusicFile() { Track = 7, Duration = 5000 });

            return library;
        }
    }
}
