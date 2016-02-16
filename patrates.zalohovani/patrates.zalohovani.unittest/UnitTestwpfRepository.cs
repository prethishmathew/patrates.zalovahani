using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using patrates.zalohovani.commons;
using patrates.zalohovani.interfaces;
using patrates.zalohovani.repository;

namespace patrates.zalohovani.unittest
{
    [TestClass]
    public class UnitTestwpfRepository
    {
        [TestMethod]
        public void savewpfRepository()
        {
            ILocalSettings tr = new   wpfLocalSettings();
            ICryption cr = new noencryption();
            ISettingRepository dummyrepository = new DummyRepository(tr, cr);
            ISettingRepository repositoryWPF = new WPFSettingRepository(tr, cr);



            repositoryWPF.saveData(dummyrepository.getdata());
            Assert.AreEqual("1", "1");
                        
        }

        [TestMethod]
        public void getwpfrepository()
        {
            ILocalSettings tr = new wpfLocalSettings();
            ICryption cr = new noencryption();
            ISettingRepository dummyrepository = new DummyRepository(tr, cr);
            ISettingRepository repositoryWPF = new WPFSettingRepository(tr, cr);
            ILocalSettings tr1 = repositoryWPF.getdata();
            ILocalSettings tr2 = dummyrepository.getdata();

            Assert.AreEqual(tr1.customerKey, tr2.customerKey);

        }
    }
}
