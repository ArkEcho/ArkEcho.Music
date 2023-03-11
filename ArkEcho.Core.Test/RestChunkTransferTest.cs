using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ArkEcho.Core.Test
{
    [TestClass]
    public class RestChunkTransferTest : FileTestBase
    {
        private const string testFileOne = "testOne.mp3";
        private const string testFileTwo = "testTwo.mp3";


        private TransferFileBase getTestFileOne()
        {
            return new TransferFileBase(CreateTestFile(testFileOne, 1));
        }

        private TransferFileBase getTestFileTwo()
        {
            return new TransferFileBase(CreateTestFile(testFileTwo, 100));
        }

        private void cleanTestFiles()
        {
            DeleteTestFile(testFileOne);
            DeleteTestFile(testFileTwo);
        }

        [TestMethod]
        public void FileCheckSum()
        {
            TransferFileBase tfbOne = getTestFileOne();
            TransferFileBase tfbTwo = getTestFileTwo();

            bool testOne = tfbOne.TestCheckSum();
            bool testTwo = tfbTwo.TestCheckSum();

            testOne.Should().BeTrue();
            testTwo.Should().BeTrue();

            tfbOne.CheckSum.Should().Be("2cb74edba754a81d121c9db6833704a8e7d417e5b13d1a19f4a52f007d644264");
            tfbTwo.CheckSum.Should().Be("7f12a2ac8cc123711b92c20e22583eaa49582c52a8c1f3050f81dd1aa6591007");

            cleanTestFiles();
        }

        [TestMethod]
        public void ChunkCreationTest()
        {
            TransferFileBase tfbOne = getTestFileOne();
            TransferFileBase tfbTwo = getTestFileTwo();

            tfbOne.Chunks.Count.Should().Be(2);
            tfbTwo.Chunks.Count.Should().Be(101);

            cleanTestFiles();
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

            cleanTestFiles();
        }

        [TestMethod]
        public async Task LoadFileWithRestTest()
        {
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
