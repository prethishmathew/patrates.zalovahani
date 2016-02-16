using patrates.zalohovani.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace patrates.zalohovani.interfaces
{
    public interface iCloud:IDisposable
    {
        //List<IcloudFileInfo> getAllfiles(int container);
        List<IcloudFileInfo> getAllfiles(int Container=0);
        void deleteFile(IcloudFileInfo fileRemote);
        void downloadFile(IcloudFileInfo fileRemote, string destinationFile);
        Task uploadFileAsyc(IcloudFileInfo fileRemote);
        //List<summaryReport> getsummaryReport();
    }
}
