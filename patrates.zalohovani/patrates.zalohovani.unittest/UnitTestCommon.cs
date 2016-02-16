using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace patrates.zalohovani.unittest
{
    [TestClass]
    public class UnitTestCommon
    {
        [TestMethod]
        public void TestMethod1()
        {
            string str = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            Assert.IsNotNull(str); 
        }
    }
}
