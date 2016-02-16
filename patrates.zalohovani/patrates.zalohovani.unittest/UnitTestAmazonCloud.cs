using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using patrates.zalohovani.amazonS3;
using patrates.zalohovani.interfaces;
using patrates.zalohovani.commons;
using patrates.zalohovani.repository;
using System.Linq;
using patrates.zalohovani.models;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace patrates.zalohovani.unittest
{
    [TestClass]
    public class UnitTestAmazonCloud
    {

        ICryption AEScipher = new AESEncryption();
        ILocalSettings tr = new wpfLocalSettings();
        ISettingRepository repositoryWPF;
        sAmazonS3 st;

        public UnitTestAmazonCloud()
        {

            repositoryWPF = new WPFSettingRepository(tr, AEScipher);
            tr = repositoryWPF.getdata();                        
            st = new sAmazonS3(tr);        
        }
          

        [TestMethod]
        public void uploadAmazon()
        {
            System.IO.FileInfo ft = new System.IO.FileInfo(@"C:\Logs\ImportManager\ErrorLog.xml");
            IcloudFileInfo fi = new cloudFileInfo ()
            { 
                localfolderName = ft.DirectoryName,
                localfileLastModifiedDate = ft.LastWriteTime.ToString(),
                localfileName = ft.Name,
                DateofBackup = DateTime.Now,
                deviceName = tr.customerName,
                 OperatingSystem = Environment.OSVersion.ToString().Split(' ').FirstOrDefault()
            };

            Task dt = st.uploadFileAsyc(fi);
            dt.Wait();
            Assert.IsTrue(dt.IsCompleted);
           
        }

        [TestMethod]
        public void getAllFiles()
        {

            List<IcloudFileInfo> tr = st.getAllfiles();
            Assert.IsTrue(tr.Count > 0);
        
        }
    }
}
