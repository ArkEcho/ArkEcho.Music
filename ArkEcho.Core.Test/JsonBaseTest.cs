using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ArkEcho.Core.Test
{
    [TestClass]
    public class JsonBaseTest
    {
        [TestMethod]
        public void TestSetAndGet()
        {
            string jsonOriginal = Properties.Resources.TestClass_txt.RemoveCRLF();

            TestClass testclass = new TestClass();

            Assert.IsTrue(testclass.LoadFromJsonString(jsonOriginal).Result);

            string jsonBased = testclass.SaveToJsonString().Result.RemoveCRLF();
            bool test = jsonOriginal.Equals(jsonBased, System.StringComparison.OrdinalIgnoreCase);

            Assert.IsTrue(test);
        }
    }
}
