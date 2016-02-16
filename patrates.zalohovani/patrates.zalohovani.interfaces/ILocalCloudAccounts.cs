using patrates.zalohovani.models;
using System;
namespace patrates.zalohovani.interfaces
{
    public interface ILocalCloudAccounts
    {
        string cloudKey1 { get; set; }
        string cloudKey2 { get; set; }
        string cloudKey3 { get; set; }
        string cloudKey4 { get; set; }
        bool isactive { get; set; }
        cloudTypes storageType { get; set; }
    }
}
