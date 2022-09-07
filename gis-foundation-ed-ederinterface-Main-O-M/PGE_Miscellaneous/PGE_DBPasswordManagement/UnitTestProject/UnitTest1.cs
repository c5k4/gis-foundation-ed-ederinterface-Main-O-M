using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using PGE_DBPasswordManagement;
namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        private TestContext testContextInstance;

        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext
        {
            get { return testContextInstance; }
            set { testContextInstance = value; }
        }

        [TestMethod]
        public void TestMethodGetPassword()
        {
            
            string result = default;
            result = ReadEncryption.GetPassword("EDGIS@EDER");
            TestContext.WriteLine(result);
            Assert.IsNotNull(result);
        }
        
        [TestMethod]
        public void TestMethodGenPassword()
        {
            string result = default;
            result = ReadEncryption.GenPassword("EDGIS@EDER");
            TestContext.WriteLine(result);
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TestMethodGetConnectionStr()
        {
            string result = "";
            result = ReadEncryption.GetConnectionStr("EDGIS@EDER");
            TestContext.WriteLine(result);
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TestMethodGetSDEPath()
        {
            string result = default;
            result = ReadEncryption.GetSDEPath("EDGIS@EDER");
            TestContext.WriteLine(result);
            Assert.IsNotNull(result);
        }
    }
}
