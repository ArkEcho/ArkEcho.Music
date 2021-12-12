using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace ArkEcho.Core.Test
{
    [TestClass]
    public class ZipCompressionTest
    {
        [TestMethod]
        public void TestZip()
        {
            string testString = Properties.Resources.TestClass_txt;
            string zippedString = ZipCompression.ZipToBase64(testString).Result;
            string unzippedString = ZipCompression.UnzipBase64(zippedString).Result;

            Assert.IsTrue(testString.Equals(unzippedString));

            byte[] testArray = testString.GetByteArray();
            byte[] zippedArray = ZipCompression.Zip(testArray).Result;
            byte[] unzippedArray = ZipCompression.Unzip(zippedArray).Result;

            Assert.IsTrue(Enumerable.SequenceEqual(testArray, unzippedArray));
        }
    }
}
