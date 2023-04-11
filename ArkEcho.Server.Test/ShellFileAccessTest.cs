using ArkEcho.Server;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ArkEcho.Core.Test
{
    [TestClass]
    public class ShellFileAccessTest : FileTestBase
    {
        [TestMethod]
        public void GetSetMusicRating()
        {
            string testFile = "TestMp3.mp3";
            string filePath = CreateMp3TestFile(testFile);

            ShellFileAccess.SetRating(filePath, 5);
            ShellFileAccess.GetRating(filePath);
            ShellFileAccess.SetRating(filePath, 4);
            ShellFileAccess.GetRating(filePath);
            ShellFileAccess.SetRating(filePath, 3);
            ShellFileAccess.GetRating(filePath);
            ShellFileAccess.SetRating(filePath, 2);
            ShellFileAccess.GetRating(filePath);
            ShellFileAccess.SetRating(filePath, 1);
            ShellFileAccess.GetRating(filePath);
            ShellFileAccess.SetRating(filePath, 0);
            ShellFileAccess.GetRating(filePath);

            DeleteTestFile(testFile);
        }
    }
}
