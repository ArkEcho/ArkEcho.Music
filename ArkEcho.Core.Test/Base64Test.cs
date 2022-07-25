using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ArkEcho.Core.Test
{
    [TestClass]
    public class EM_StringTest
    {
        [TestMethod]
        public void ConvertBase64()
        {
            string testText = "Lorem Ipsum 1234 Test";
            string copyTestText = testText.ToBase64();
            copyTestText = copyTestText.FromBase64();

            Assert.IsTrue(copyTestText.Equals(testText, System.StringComparison.OrdinalIgnoreCase));
        }
    }
}
