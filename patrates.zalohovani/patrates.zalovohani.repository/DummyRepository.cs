using patrates.zalohovani.commons;
using patrates.zalohovani.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace patrates.zalohovani.repository
{
    public class DummyRepository: ISettingRepository
    {

        ILocalSettings _localSetting;
        
        private const string _ZALOHOVANISETTINGS = "patrates.ZALOHOVANISETTINGS.xml";
        private const string _folder = "Zalohovani";
        public ICryption iCryption;
        string settingsDirectory;

        public DummyRepository(ILocalSettings tr, ICryption cryption)
        {
            this._localSetting = tr;
            this.iCryption = cryption;
            this.settingsDirectory = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
        }

        public ILocalSettings getdata()
        {
            this._localSetting.automate = false;


            this._localSetting.icloudSettings = new List<ILocalCloudAccounts>();
            this._localSetting.icloudSettings.Add(new LocalCloudAccounts
                                            { storageType = models.cloudTypes.Amazon, 
                                                cloudKey1 = "amazonKey1", 
                                                cloudKey2 = "amazonKey2", 
                                                isactive = true});

            this._localSetting.icloudSettings.Add(new LocalCloudAccounts
            {
                storageType = models.cloudTypes.Azure,
                cloudKey1 = "AzureKey1",
                cloudKey2 = "AzureKey2",
                isactive = false
            });


            this._localSetting.frequeny = models.backupfrequency.Daily;
            
            this._localSetting.monWeek = true;
            this._localSetting.tueWeek = true;
            this._localSetting.wedWeek = true;
            this._localSetting.thuWeek = true;
            this._localSetting.friWeek = true;
            this._localSetting.satWeek = true;
            this._localSetting.sunWeek = true;

            this._localSetting.timeofRun = DateTime.Now;
            this._localSetting.foldersToBackUp = new List<string>();
            this._localSetting.foldersToBackUp.Add("mydocuments");
            this._localSetting.foldersToBackUp.Add("mypictures");

            this._localSetting.customerName = "My Windows 8";
            this._localSetting.customerKey = "123456";


            return this._localSetting;
        
        }


        public void saveData(ILocalSettings tr)
        {
            throw new NotImplementedException();
        }
    }
}
