using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ArkEcho.Core.Test
{
    [TestClass]
    public class RestChunkTransferTest
    {
        private const string testFolder = @"C:\Work\Test\TestFiles\";

        private const string testFileOne = testFolder + "testOne.mp3";
        private const string testFileTwo = testFolder + "testTwo.mp3";

        private TransferFileBase getTestFileOne()
        {
            return new TransferFileBase(testFolder + "Doom Eternal - ETERNAL GAINS - GYM MIX.mp3");
        }

        private TransferFileBase getTestFileTwo()
        {
            return new TransferFileBase(testFolder + "01 Papercut.mp3");
        }

        private void cleanTestFiles()
        {
            if (File.Exists(testFileOne))
                File.Delete(testFileOne);

            if (File.Exists(testFileTwo))
                File.Delete(testFileTwo);
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

        [TestMethod]
        public async Task LoadFileWithRestTest()
        {
            cleanTestFiles();

            MemoryStream streamOne = null;
            MemoryStream streamTwo = null;
            FileStream fs = null;

            try
            {
                TransferFileBase tfbOne = getTestFileOne();
                TransferFileBase tfbTwo = getTestFileTwo();

                TestRest rest = new TestRest(new List<TransferFileBase> { tfbOne, tfbTwo }, false);

                streamOne = await rest.GetFile(tfbOne);
                streamTwo = await rest.GetFile(tfbTwo);

                Assert.IsTrue(streamOne != null);
                Assert.IsTrue(streamTwo != null);

                using (fs = new FileStream(testFileOne, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
                {
                    streamOne.Seek(0, SeekOrigin.Begin);
                    streamOne.CopyTo(fs);
                }

                using (fs = new FileStream(testFileTwo, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
                {
                    streamTwo.Seek(0, SeekOrigin.Begin);
                    streamTwo.CopyTo(fs);
                }

                Assert.IsTrue(File.Exists(testFileOne));
                Assert.IsTrue(File.Exists(testFileTwo));

                string checkSumOne = TransferFileBase.GetCheckSum(testFileOne);
                string checkSumTwo = TransferFileBase.GetCheckSum(testFileTwo);

                // We cant use "TestCheckSum()" because we created a new file with another name
                Assert.IsTrue(tfbOne.CheckSum == checkSumOne);
                Assert.IsTrue(tfbTwo.CheckSum == checkSumTwo);
            }
            catch
            {
                Assert.IsTrue(false, "Exception in RunningTest!");
            }
            finally
            {
                fs?.Dispose();

                streamOne?.Dispose();
                streamTwo?.Dispose();

                cleanTestFiles();
            }
        }
    }
}
