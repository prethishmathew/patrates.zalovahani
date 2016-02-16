using patrates.zalohovani.models;
using System;
namespace patrates.zalohovani.interfaces
{
    public interface IcloudFileInfo
    {
        DateTime DateofBackup { get; set; }
        string CloudFileName { get; set; }
        string deviceName { get; set; }
        string localfileLastModifiedDate { get; set; }
        string localfileName { get; set; }
        long fileSize { get; set; }
        string localfolderName { get; set; }
        string OperatingSystem { get; set; }
        cloudTypes storageType { get; set; }
    }
}
