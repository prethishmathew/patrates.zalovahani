using patrates.zalohovani.interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace patrates.zalohovani.interfaces
{
    public interface IcloudRepository:IDisposable
    {
        void downloadFiles(IcloudFileInfo fileinfo);
        void downloadFilesAsync(IEnumerable<IcloudFileInfo> filelist,string downloadFolder);
        List<IcloudFileInfo> getFilesList();
        Task uploadFilesAsync();
    }
}
