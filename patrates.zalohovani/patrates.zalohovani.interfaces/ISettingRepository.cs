
using System;
namespace patrates.zalohovani.interfaces
{
    public interface ISettingRepository
    {
        ILocalSettings getdata();
        void saveData(ILocalSettings tr);
    }
}
