using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using patrates.zalohovani.interfaces;
using System.IO;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Xml;
using patrates.zalohovani.commons;

namespace patrates.zalohovani.repository
{
    public class WPFSettingRepository : ISettingRepository
    {

        ILocalSettings _localSetting;
        ICryption _iCryption;

        private const string _ZALOHOVANISETTINGS = "patrates.zalohovani.zml";
        private const string _zalovahoaniCloudDts = "patrates.zalohovanistore.zml";
        private const string _folder = "Zalohovani";
        
        string settingsDirectory;

        public WPFSettingRepository(ILocalSettings tr, ICryption cryption)
        {
            this._localSetting = tr;
            this._iCryption = cryption;
            this.settingsDirectory = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
        }

        public ILocalSettings getdata()
        {
            string settingsDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string path = Path.Combine(settingsDirectory, _folder, _ZALOHOVANISETTINGS);
            wpfLocalSettings deserializedSetting;
            if (!File.Exists(path)) return null;

            FileStream fs = new FileStream(path,
                    FileMode.Open);
            {
                using (XmlDictionaryReader reader =
                    XmlDictionaryReader.CreateTextReader(fs, new XmlDictionaryReaderQuotas()))
                {
                    DataContractSerializer ser = new DataContractSerializer(typeof(wpfLocalSettings), new Type[] { typeof(LocalCloudAccounts) });
                    deserializedSetting =
                        (wpfLocalSettings)ser.ReadObject(reader, true);

                    List<ILocalCloudAccounts> cloudNew = new List<ILocalCloudAccounts>();
                    foreach (ILocalCloudAccounts str in deserializedSetting.icloudSettings)
                    {
                        if (!string.IsNullOrEmpty(str.cloudKey1)) { str.cloudKey1 = _iCryption.decrypt(str.cloudKey1); }
                        if (!string.IsNullOrEmpty(str.cloudKey2)) { str.cloudKey2 = _iCryption.decrypt(str.cloudKey2); }
                        if (!string.IsNullOrEmpty(str.cloudKey3)) { str.cloudKey3 = _iCryption.decrypt(str.cloudKey3); }
                        if (!string.IsNullOrEmpty(str.cloudKey4)) { str.cloudKey4 = _iCryption.decrypt(str.cloudKey4); }
                        cloudNew.Add(str);
                    }

                    deserializedSetting.icloudSettings = cloudNew;
                }
            }
            return deserializedSetting;

            

            
        
        }
        
        public void saveData(ILocalSettings tr)
        {
            string settingsDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            List<ILocalCloudAccounts> cloudNew = new List<ILocalCloudAccounts>();
            
            foreach (ILocalCloudAccounts str in tr.icloudSettings)
            {
                 
                if (!string.IsNullOrEmpty(str.cloudKey1)) {str.cloudKey1 = _iCryption.encrypt(str.cloudKey1);}
                if (!string.IsNullOrEmpty(str.cloudKey2)) {str.cloudKey2 = _iCryption.encrypt(str.cloudKey2);}
                if (!string.IsNullOrEmpty(str.cloudKey3)){str.cloudKey3 = _iCryption.encrypt(str.cloudKey3);}
                if (!string.IsNullOrEmpty(str.cloudKey4)){str.cloudKey4 = _iCryption.encrypt(str.cloudKey4);}
                cloudNew.Add(str);
            }

            tr.icloudSettings = cloudNew;

            Type t = tr.GetType();
            DataContractSerializer serializer = new DataContractSerializer(t ,  new Type[] { typeof(LocalCloudAccounts) });            
            StringWriter sw = new StringWriter();
            XmlTextWriter xw = new XmlTextWriter(sw);
            serializer.WriteObject(xw, tr);
            string dw = sw.ToString();
            
            
            string path = Path.Combine(settingsDirectory, _folder);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            path = Path.Combine(settingsDirectory, _folder,_ZALOHOVANISETTINGS);
            System.IO.File.WriteAllText(path, dw);
        }

        

       


    }
}
