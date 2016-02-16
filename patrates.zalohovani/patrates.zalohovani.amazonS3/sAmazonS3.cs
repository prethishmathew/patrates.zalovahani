using patrates.zalohovani.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using System.IO;
using patrates.zalohovani.models;
using System.Threading.Tasks;
using Amazon.S3.Util;
using Amazon.S3.Model.Internal;
using Amazon.ElasticMapReduce.Model;



namespace patrates.zalohovani.amazonS3
{
    public class sAmazonS3:iCloud,IDisposable

    {

        private string bucketname;
        private IAmazonS3 client;
        private ILocalCloudAccounts _settings;
        private List<string> _listofFoldersToBackup;

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
                if (client != null)
                {
                    client.Dispose();
                    client = null;
                }
            }
          
        }

        public sAmazonS3(ILocalSettings settings)
        {
           // Amazon.Runtime.BasicAWSCredentials credentials = new Amazon.Runtime.BasicAWSCredentials(settings.cloudKey1, settings.cloudKey2);
            _listofFoldersToBackup = settings.foldersToBackUp;
            _settings = settings.icloudSettings.FirstOrDefault(p => p.storageType == patrates.zalohovani.models.cloudTypes.Amazon); 
            AmazonS3Config S3Config = new AmazonS3Config()
            {
                ServiceURL = "s3.amazonaws.com",
                RegionEndpoint = Amazon.RegionEndpoint.USEast1
            };

            try
            {

                this.client = new AmazonS3Client(_settings.cloudKey1, _settings.cloudKey2, S3Config);
            }
            catch (AmazonS3Exception err)
            {

                throw new Exception(err.Message);
            }
            catch (Exception err)
            {

                throw new Exception(err.Message);
            }
            
            this.bucketname = _settings.cloudKey3;
            Boolean istrue = AmazonS3Util.DoesS3BucketExist(client, this.bucketname);
            
            if (!(istrue))
            {
                _CreateABucket(this.bucketname);
            }

            
        }
        /// <summary>
        /// Downloads a file 
        /// </summary>
        /// <param name="accessKey">The AWSAccessKey </param>
        /// <param name="SecretKey">The AWSSecretKey </param>
        /// <param name="StorageAccount">The AS3 Bucket to be used </param>
        /// <param name="DataRegion">The AS3 Region where data is stored. Valid Values are USE1, USW1,USW2 or Null</param>
        /// <returns></returns>
        public void downloadFile(IcloudFileInfo file, string dest)
        {
            //var di = new DirectoryInfo(dest);
            //di.Attributes |= FileAttributes.Normal;
            
            dest = Path.Combine(dest, file.localfileName);
            if (File.Exists(dest))
            { 
                File.Delete(dest); 
            }
            using (client )
            {
                GetObjectRequest getObjectRequest = new GetObjectRequest
                {
                    BucketName = this.bucketname,
                    Key = file.deviceName + @"/" + file.CloudFileName 
                };
                using (GetObjectResponse getObjectResponse = client.GetObject(getObjectRequest))
                {
                    
                    {
                        using (Stream s = getObjectResponse.ResponseStream)
                        {
                            using (FileStream fs = new FileStream(dest, FileMode.Create, FileAccess.ReadWrite))
                            {
                                byte[] data = new byte[304087];
                                int bytesRead = 0;
                                do
                                {
                                    bytesRead = s.Read(data, 0, data.Length);
                                    fs.Write(data, 0, bytesRead);
                                }
                                while (bytesRead > 0);
                                fs.Flush();
                            }
                        }
                    }
                }
            }
        }

        public List<IcloudFileInfo> getAllfiles(int container = 0)
        {
            List<IcloudFileInfo> listOFFiles = new List<IcloudFileInfo>();

            using (client)
            {

                
                ListObjectsRequest request = new ListObjectsRequest
                {
                    BucketName = this.bucketname,
                    MaxKeys = 30

                };

                if (container > 0)
                {
                    request.Prefix = container.ToString() + "/";                    
                }

                do
                {

                    ListObjectsResponse response = client.ListObjects(request);
                    
                    foreach (S3Object entry in response.S3Objects)
                    {
                            if (entry.Key == container.ToString() + "/")
                            {
                                continue ;
                            }

                            IcloudFileInfo rd = new cloudFileInfo();
                            rd.DateofBackup = entry.LastModified;
                            rd.CloudFileName = entry.Key.Split('/')[1];
                            rd.fileSize = entry.Size;
                            
                            GetObjectMetadataRequest metaRequest = new GetObjectMetadataRequest
                            {
                                BucketName = this.bucketname,
                                Key = entry.Key
                            };
                            try
                            {
                             // A try catch is needed to skip all keys that don't have valid metadata;

                            GetObjectMetadataResponse responseMeta = client.GetObjectMetadata(metaRequest);                            
                            rd.fileSize = entry.Size;
                            rd.deviceName = responseMeta.Metadata["x-amz-meta-devicename"];
                            rd.localfileName = responseMeta.Metadata["x-amz-meta-filename"];
                            rd.localfileLastModifiedDate = responseMeta.Metadata["x-amz-meta-modifieddate"].ToString();                                                        
                            rd.localfolderName = responseMeta.Metadata["x-amz-meta-foldername"].ToString();
                            rd.OperatingSystem = responseMeta.Metadata["x-amz-meta-operatingsystem"].ToString();
                             
                            listOFFiles.Add(rd);
                            }
                            catch (Exception)
                            {
                                continue;
                            }
                            
                        
                    }
                    // If response is truncated, set the marker to get the next 
                    // set of keys.
                    if (response.IsTruncated)
                    {
                        request.Marker = response.NextMarker;
                    }
                    else
                    {
                        request = null;
                    }
                } while (request != null);
             }
            return listOFFiles;
            

        }
        public async Task uploadFileAsyc(IcloudFileInfo fr)
        {
            //Create  A New Client For every request,
/*
            AmazonS3Config S3Config = new AmazonS3Config()
            {
                ServiceURL = "s3.amazonaws.com",
                RegionEndpoint = Amazon.RegionEndpoint.USEast1
            };
            */
            try
            {

              //  AmazonS3Client clientUpload = new AmazonS3Client(_settings.cloudKey1, _settings.cloudKey2, S3Config);

                TransferUtility fileTransferUtility = new
                            TransferUtility(this.client);
                fr.CloudFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + "_" + Guid.NewGuid().ToString("N") + "_" + Path.GetFileName(fr.localfileName);
                TransferUtilityUploadRequest fileTransferUtilityRequest =
                    new TransferUtilityUploadRequest
                    {

                        BucketName = (this.bucketname),
                        FilePath = (Path.Combine(fr.localfolderName, fr.localfileName)),
                        Key = (fr.deviceName + @"/" + fr.CloudFileName),
                        //PartSize = (1457),
                        CannedACL = S3CannedACL.AuthenticatedRead

                    };
                fileTransferUtilityRequest.Metadata["FileName"] = Path.GetFileName(fr.localfileName);
                fileTransferUtilityRequest.Metadata["DateCreated"] = fr.DateofBackup.ToString();
                fileTransferUtilityRequest.Metadata["FolderName"] = fr.localfolderName;
                fileTransferUtilityRequest.Metadata["ModifiedDate"] = fr.localfileLastModifiedDate;
                fileTransferUtilityRequest.Metadata["DeviceName"] = fr.deviceName;
                fileTransferUtilityRequest.Metadata["OperatingSystem"] = fr.OperatingSystem;

                await fileTransferUtility.UploadAsync (fileTransferUtilityRequest);
                
            }
            catch (AmazonS3Exception err)
            {

                throw new Exception(err.Message);
            }
            catch (Exception err)
            {

                throw new Exception(err.Message);
            }
           

        }
  /*      public List<IcloudFileInfo> getAllfiles()
        {
            using (client)
            {
                List<IcloudFileInfo> listOFFiles = new List<IcloudFileInfo>();
                ListObjectsRequest request = new ListObjectsRequest
                {
                    BucketName = this.bucketname,
                    MaxKeys = 30

                };

                do
                {

                    ListObjectsResponse response = client.ListObjects(request);
                    foreach (S3Object entry in response.S3Objects)
                    {
                        IcloudFileInfo rd = new cloudFileInfo();
                        rd.DateofBackup = entry.LastModified;
                        rd.deviceName = entry.Key.Split('/')[0];
                        rd.CloudFileName = entry.Key.Split('/')[1];
                        rd.fileSize = entry.Size;
                        GetObjectMetadataRequest metaRequest = new GetObjectMetadataRequest
                        {
                            BucketName = this.bucketname,
                            Key = entry.Key
                        };
                        GetObjectMetadataResponse responseMeta = client.GetObjectMetadata(metaRequest);
                        rd.localfileName = responseMeta.Metadata["FileName"];
                        rd.localfileLastModifiedDate = responseMeta.Metadata["ModifiedDate"].ToString();
                        rd.localfolderName = responseMeta.Metadata["FolderName"].ToString();
                        
                        listOFFiles.Add(rd);

                    }

                    // If response is truncated, set the marker to get the next 
                    // set of keys.
                    if (response.IsTruncated)
                    {

                        request.Marker = response.NextMarker;
                    }
                    else
                    {
                        request = null;
                    }
                } while (request != null);
                return listOFFiles;
            }



        }
   * */
        public void deleteFile(IcloudFileInfo fr)
        {
            DeleteObjectRequest deleteObjectRequest =
            new DeleteObjectRequest
            {
                BucketName = (this.bucketname),
                Key = (fr.deviceName + @"/" + (fr.CloudFileName))
            };
            using (client )
            {
                client.DeleteObject(deleteObjectRequest);
            }

        }

        private Boolean _CreateABucket(string bucketName)
        {
            try
            {
                PutBucketRequest putRequest1 = new PutBucketRequest
                {
                    BucketName = bucketName,
                    UseClientRegion = true
                };

                PutBucketResponse response1 = client.PutBucket(putRequest1);
                return true;
            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                if (amazonS3Exception.ErrorCode != null &&
                    (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId")
                    ||
                    amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
                {
                    throw new AmazonS3Exception(amazonS3Exception.Message);
                }
                else
                {
                    throw new AmazonS3Exception(amazonS3Exception.Message);
                }
            }

            

        }
    
    }
}
