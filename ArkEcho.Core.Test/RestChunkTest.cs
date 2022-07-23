using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ArkEcho.Core.Test
{
    [TestClass]
    public class RestChunkTest
    {
        private TransferFileBase getTestFileOne()
        {
            return new TransferFileBase(@"C:\Work\Test\TestFiles\Doom Eternal - ETERNAL GAINS - GYM MIX.mp3");
        }

        private TransferFileBase getTestFileTwo()
        {
            return new TransferFileBase(@"C:\Work\Test\TestFiles\01 Papercut.mp3");
        }

        [TestMethod]
        public void FileCheckSum()
        {
            TransferFileBase tfbOne = getTestFileOne();
            TransferFileBase tfbTwo = getTestFileTwo();

            bool testOne = tfbOne.TestCheckSum();
            bool testTwo = tfbTwo.TestCheckSum();

            Assert.IsTrue(tfbOne.CheckSum != tfbTwo.CheckSum);
            Assert.IsTrue(testOne);
            Assert.IsTrue(testTwo);
        }

        [TestMethod]
        public void ChunkCreationTest()
        {
            TransferFileBase tfbOne = getTestFileOne();
            TransferFileBase tfbTwo = getTestFileTwo();

            Assert.IsTrue(tfbOne.Chunks.Count == 7); // 100mb / 15mb ~ 7 Chunks
            Assert.IsTrue(tfbTwo.Chunks.Count == 1); // 4.3 / 15mb ~ 1 Chunks
        }

        [TestMethod]
        public void ChunkPositionAndSizesTest()
        {
            TransferFileBase tfbOne = getTestFileOne();

            long positionSize = 0;
            foreach (TransferFileBase.FileChunk chunk in tfbOne.Chunks)
            {
                Assert.IsTrue(positionSize == chunk.Position);
                positionSize += chunk.Size;
            }

            Assert.IsTrue(positionSize == tfbOne.FileSize);
        }
    }
}
