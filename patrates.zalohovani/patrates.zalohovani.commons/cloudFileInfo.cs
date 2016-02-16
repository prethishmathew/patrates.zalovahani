using patrates.zalohovani.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace patrates.zalohovani.models
{
    public class cloudFileInfo : IcloudFileInfo
    {
        public string localfolderName { get; set; }
        public string localfileName { get; set; }
        public string localfileLastModifiedDate { get; set; }
        public string CloudFileName { get; set; }
        public DateTime DateofBackup { get; set; }
        public long fileSize { get; set; }
        public string deviceName { get; set; }
        public cloudTypes storageType { get; set; }
        public string OperatingSystem { get; set; }
    }
}
