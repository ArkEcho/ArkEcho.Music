using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ArkEcho.Core.Test
{
    [TestClass]
    public class MusicTestBase
    {
        public MusicLibrary GetTestMusicLibrary()
        {
            MusicLibrary library = new MusicLibrary();

            library.MusicFiles.Add(new MusicFile() { Duration = 2500 });
            library.MusicFiles.Add(new MusicFile() { Duration = 3000 });
            library.MusicFiles.Add(new MusicFile() { Duration = 3500 });
            library.MusicFiles.Add(new MusicFile() { Duration = 1000 });
            library.MusicFiles.Add(new MusicFile() { Duration = 2000 });
            library.MusicFiles.Add(new MusicFile() { Duration = 4000 });
            library.MusicFiles.Add(new MusicFile() { Duration = 5000 });

            return library;
        }
    }
}
