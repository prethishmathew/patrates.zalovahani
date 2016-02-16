using patrates.zalohovani.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using patrates.zalohovani.interfaces;

namespace patrates.zalohovani.commons
{
    public class LocalCloudAccounts : ILocalCloudAccounts
    {
        public cloudTypes storageType { get; set; }
        public string cloudKey1 { get; set; }
        public string cloudKey2 { get; set; }
        public string cloudKey3 { get; set; }
        public string cloudKey4 { get; set; }
        public Boolean isactive { get; set; }
    }
}
