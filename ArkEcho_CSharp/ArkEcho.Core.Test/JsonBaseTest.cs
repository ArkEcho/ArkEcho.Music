using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ArkEcho.Core.Test
{
    [TestClass]
    public class JsonBaseTest
    {
        [TestMethod]
        public void TestSetAndGet()
        {
            string jsonOriginal = Properties.Resources.TestClass_txt;
            TestClass testclass = new TestClass();

            string jsonBased = testclass.Load(jsonOriginal);

            Assert.IsTrue(jsonOriginal.Equals(jsonBased, System.StringComparison.OrdinalIgnoreCase));
        }
    }
}
