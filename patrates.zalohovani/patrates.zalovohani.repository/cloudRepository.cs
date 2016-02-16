using patrates.zalohovani.amazonS3;

using patrates.zalohovani.Model;
using patrates.zalohovani.models;
using patrates.zalohovani.interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace patrates.zalohovani.repository
{
    public class cloudRepository : IcloudRepository,IDisposable
    {
        ILocalSettings _settings;
        List<iCloud> _lcloudList;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources
                if (_lcloudList != null)
                {
                    foreach (var item in _lcloudList)
                    {
                        item.Dispose();
                        
                    }
                    _lcloudList = null;
                }
            }

        }
        public cloudRepository(ILocalSettings tr)
        {
            this._lcloudList = new List<iCloud>();
            this._settings = tr;
        }


        public List<IcloudFileInfo> getFilesList()
        {
            List<IcloudFileInfo> cloudfiles = new List<IcloudFileInfo>();

            foreach(var cloudService in this._settings.icloudSettings)
            {

                iCloud service = this.getService(cloudService.storageType);
                if (service != null)
                {

                    cloudfiles.AddRange(service.getAllfiles());
                }


            }

            return cloudfiles;
        
        }

        public void downloadFiles(IcloudFileInfo fileinfo)
        {
            throw new NotImplementedException();
        }


        
        public async void downloadFilesAsync(IEnumerable<IcloudFileInfo> filelist, string downloadFolder)
        {
                   
            foreach (var file in filelist)
            {
                iCloud service = null;    
                switch (file.storageType)
                {
                    case cloudTypes.Amazon:
                        service = new sAmazonS3(this._settings);
                        break;
                    case cloudTypes.Azure:
                        throw new NotImplementedException();
                }

                await Task.Run(() => service.downloadFile(file, downloadFolder));
                
            }


        }

        private iCloud  getService(cloudTypes storage)
        {
            iCloud service = null;
            switch (storage)
            {
                case cloudTypes.Amazon:
                        service = new sAmazonS3(this._settings);                        
                        return service;
                case cloudTypes.Azure:
                    //throw new NotImplementedException();
                    return null;
            }
            
            return service;

        }
        public async Task uploadFilesAsync()
        {
            string systemO = null; 
            try
            {

                systemO = Environment.OSVersion.ToString().Split(' ').FirstOrDefault();
            }
            catch (Exception)
            {

                throw;
            }
            
            var defaultCloud = this._settings.icloudSettings.FirstOrDefault(p => p.isactive == true);
            iCloud _iCloud = null;
            try
            {

                
                switch (defaultCloud.storageType)
                {

                    case cloudTypes.Amazon:
                        _iCloud = new sAmazonS3(this._settings);
                        break;
                    case cloudTypes.Azure:
                        throw (new NotImplementedException());
                    default:
                        throw (new NotImplementedException());

                }

                foreach (string item in this._settings.foldersToBackUp)
                {
                    try
                    {
                        DirectoryInfo di = new DirectoryInfo(item);

                        foreach (FileInfo fi in di.GetFiles("*", SearchOption.AllDirectories))
                        {
                            IcloudFileInfo cloudFi = new cloudFileInfo()
                            {
                                DateofBackup = DateTime.Now,
                                deviceName = this._settings.customerName,
                                localfileName = fi.Name,
                                localfileLastModifiedDate = fi.LastWriteTime.ToString(),
                                localfolderName = fi.DirectoryName,
                                OperatingSystem = systemO
                            };

                            await _iCloud.uploadFileAsyc(cloudFi);

                        }
                    }
                    catch (Exception ex)
                    {
                        throw;
                        //Write ErrorLogging
                    }


                }

                
            }

            catch (Exception ex)
            {

                throw;
            }
            finally
            {
                if (_iCloud != null) _iCloud.Dispose();                           
            }

        }


        private void UploadFiles(string fi)
        {
            throw new NotImplementedException();
        }
       

    }
}
