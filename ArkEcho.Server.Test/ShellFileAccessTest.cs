using ArkEcho.Core.Test;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ArkEcho.Server.Test
{
    [TestClass]
    public class ShellFileAccessTest : FileTestBase
    {
        [TestMethod]
        public void GetSetMusicRating()
        {
            string testFile = "TestMp3.mp3";
            string filePath = CreateMp3TestFile(testFile);

            for (int i = 5; i <= 0; i--)
            {
                ShellFileAccess.SetRating(filePath, i);
                ShellFileAccess.GetRating(filePath).Should().Be(i);
            }

            DeleteTestFile(testFile);
        }
    }
}
