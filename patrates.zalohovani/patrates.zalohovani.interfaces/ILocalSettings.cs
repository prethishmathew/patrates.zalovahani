using patrates.zalohovani.models;
using System;
using System.Collections.Generic;
namespace patrates.zalohovani.interfaces
{
    public interface ILocalSettings
    {
               
        bool automate { get; set; }
        
        string customerKey { get; set; }
        string customerName { get; set; }
       
        System.Collections.Generic.List<string> foldersToBackUp { get; set; }
        backupfrequency frequeny { get; set; }
        bool friWeek { get; set; }
        
        bool monWeek { get; set; }
        bool satWeek { get; set; }
        bool sunWeek { get; set; }
        bool thuWeek { get; set; }
        DateTime timeofRun { get; set; }
        bool tueWeek { get; set; }
        bool wedWeek { get; set; }
        List<ILocalCloudAccounts> icloudSettings { get; set; }
            }

}
