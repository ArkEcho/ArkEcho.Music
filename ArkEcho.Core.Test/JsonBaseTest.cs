using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace ArkEcho.Core.Test
{
    [TestClass]
    public class JsonBaseTest
    {
        private TestClass getTestClass()
        {

            string jsonOriginal = Properties.Resources.TestClass_txt.RemoveCRLF();

            TestClass testclass = new TestClass();
            bool result = testclass.LoadFromJsonString(jsonOriginal).Result;

            if (!result)
                return null;
            else
                return testclass;
        }

        [TestMethod]
        public void LoadClassFromJson()
        {
            string jsonOriginal = Properties.Resources.TestClass_txt.RemoveCRLF();

            TestClass testclass = getTestClass();

            Assert.IsTrue(testclass != null);

            Assert.IsTrue(testclass.Boolean);
            Assert.IsTrue(testclass.Int == 123);
            Assert.IsTrue(testclass.Double == 123.45);
            Assert.IsTrue(testclass.String == "tEsTStRing");
            Assert.IsTrue(testclass.Guid == Guid.Parse("936da01f-9abd-4d9d-80c7-02af85c822a8"));
            Assert.IsTrue(testclass.DateTime == DateTime.Parse("2030-12-12T12:23:45"));
            Assert.IsTrue(testclass.Uri == new Uri("C:/Test/Folder"));
            Assert.IsTrue(testclass.Enum == TestClass.TestEnum.Autumn);

            Assert.IsTrue(testclass.ListInt.Count == 2);
            Assert.IsTrue(testclass.ListInt[1] == 678);
            Assert.IsTrue(testclass.ArrayInt.Length == 2);
            Assert.IsTrue(testclass.ArrayInt[1] == 324);

            Assert.IsTrue(testclass.ListClass.Count == 2);
            Assert.IsTrue(testclass.ListClass[1].Int == 260);
            Assert.IsTrue(testclass.ArrayClass.Length == 2);
            Assert.IsTrue(testclass.ArrayClass[1].Int == 260);
        }

        [TestMethod]
        public void SaveClassToJson()
        {
            string jsonOriginal = Properties.Resources.TestClass_txt.RemoveCRLF();

            TestClass testclass = getTestClass();

            string jsonBased = testclass.SaveToJsonString().Result.RemoveCRLF();
            bool test = jsonOriginal.Equals(jsonBased, System.StringComparison.CurrentCulture);

            Assert.IsTrue(test);
        }
    }
}
