using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using patrates.zalohovani.interfaces;
using patrates.zalohovani.commons;
using System.Linq;
using patrates.zalohovani.repository;

namespace patrates.zalohovani.unittest
{
    [TestClass]
    public class UnitTestAESEncryption
    {
        [TestMethod]
        public void AESDecryptEncrypt()

        {
            
            ICryption AEScipher = new AESEncryption();
            string guid = Guid.NewGuid().ToString();
            string Script = AEScipher.encrypt(guid);
            string decryptdata = AEScipher.decrypt(Script);


            Assert.AreEqual(guid, decryptdata);
        }



        [TestMethod]
        public void AESRunWPFTest()
        {

            ICryption AEScipher = new AESEncryption();


            ILocalSettings tr = new wpfLocalSettings();
            ILocalSettings trOrginal = new wpfLocalSettings();
            ICryption cr = new noencryption();
            ISettingRepository dummyrepository = new DummyRepository(tr, cr);
            ISettingRepository dummyrepositoryorg = new DummyRepository(trOrginal, cr);
            ISettingRepository repositoryWPF = new WPFSettingRepository(tr, AEScipher);

            tr = dummyrepository.getdata();
            trOrginal = dummyrepositoryorg.getdata(); 
            repositoryWPF.saveData(tr);

            ILocalSettings returnData = repositoryWPF.getdata();
            Boolean isSame = true;
            int i = 0;
            //isSame = Enumerable.SequenceEqual(tr.icloudSettings.OrderBy(t => t), returnData.icloudSettings.OrderBy(t => t));
            isSame = trOrginal.icloudSettings.SequenceEqual(returnData.icloudSettings);
            i = trOrginal.icloudSettings.Except(returnData.icloudSettings).ToList().Count;
            Assert.Equals(i, 0);
        }


    }
}
