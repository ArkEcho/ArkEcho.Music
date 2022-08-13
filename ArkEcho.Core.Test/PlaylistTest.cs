using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ArkEcho.Core.Test
{
    [TestClass]
    public class PlaylistTest : MusicTestBase
    {
        [TestMethod]
        public void GetDurationTest()
        {
            // Arrange
            MusicLibrary library = GetTestMusicLibrary();
            Playlist playlist = new Playlist();

            library.MusicFiles.ForEach(x => playlist.MusicFiles.Add(x.GUID));

            // Act
            long duration = playlist.GetDurationInMilliseconds(library);

            // Assert
            Assert.AreEqual(47500, duration);
        }
    }
}
