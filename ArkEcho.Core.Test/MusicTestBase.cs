using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace ArkEcho.Core.Test
{
    [TestClass]
    public partial class MusicTestBase
    {
        public MusicLibrary GetTestMusicLibrary()
        {
            MusicLibrary library = new MusicLibrary();

            library.MusicFiles.Add(new MusicFile() { Performer = "Track", Title = "One", Track = 1, Duration = 20000, GUID = Guid.NewGuid() });
            library.MusicFiles.Add(new MusicFile() { Performer = "Track", Title = "Two", Track = 2, Duration = 3000, GUID = Guid.NewGuid() });
            library.MusicFiles.Add(new MusicFile() { Performer = "Track", Title = "Three", Track = 3, Duration = 3500, GUID = Guid.NewGuid() });
            library.MusicFiles.Add(new MusicFile() { Performer = "Track", Title = "Four", Track = 4, Duration = 10000, GUID = Guid.NewGuid() });
            library.MusicFiles.Add(new MusicFile() { Performer = "Track", Title = "Five", Track = 5, Duration = 2000, GUID = Guid.NewGuid() });
            library.MusicFiles.Add(new MusicFile() { Performer = "Track", Title = "Six", Track = 6, Duration = 4000, GUID = Guid.NewGuid() });
            library.MusicFiles.Add(new MusicFile() { Performer = "Track", Title = "Seven", Track = 7, Duration = 5000, GUID = Guid.NewGuid() });

            return library;
        }
    }
}
