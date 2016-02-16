using patrates.zalohovani.interfaces;
using patrates.zalohovani.models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace patrates.zalohovani.commons
{
    public class wpfLocalSettings : ILocalSettings
    {               
        public string customerKey { get; set; }
        public string customerName { get; set; }
        public List<string> foldersToBackUp { get; set; }        
        public List<ILocalCloudAccounts> icloudSettings { get; set; }      
        public bool automate { get; set; }
        public backupfrequency frequeny { get; set; }        
        public DateTime timeofRun { get; set; }
        public bool sunWeek { get; set; }
        public bool monWeek { get; set; }
        public bool tueWeek { get; set; }
        public bool wedWeek { get; set; }
        public bool thuWeek { get; set; }
        public bool friWeek { get; set; }
        public bool satWeek { get; set; }
        

    }
}
