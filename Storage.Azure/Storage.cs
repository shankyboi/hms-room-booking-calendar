using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.File;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Auth;
using System.IO;

using Storage.Interface;
using Microsoft.Extensions.Configuration;
using System.Reflection.Metadata;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using MimeMapping;
namespace Storage.Azure
{
    public class Storage : IStorage
    {

        private string baseURL;
        private string AccountName;
        private string AccountKey;
        private string ImageContainer;
        private string folderName;
        private string blobconnectionstring;
        static BlobContainerClient blobContainer;
        public Storage(IConfiguration configuration)
        {
            baseURL = configuration.GetSection("AzureBlobStorageSetttings:baseURL").Value;
            AccountName = configuration.GetSection("AzureBlobStorageSetttings:AccountName").Value;
            AccountKey = configuration.GetSection("AzureBlobStorageSetttings:AccountKey").Value;
            ImageContainer = configuration.GetSection("AzureBlobStorageSetttings:ImageContainer").Value;
            folderName = configuration.GetSection("AzureBlobStorageSetttings:folderName").Value;
            blobconnectionstring = configuration.GetSection("AzureBlobStorageSetttings:ConnectionString").Value;


        }
        public async Task<string> UploadFileAsync(Stream data, string ContentType, string fileName)
        {

            String returnFileName = "";

            using (var memoryStream = new MemoryStream())
            {
                data.CopyTo(memoryStream);
                memoryStream.ToArray();
                memoryStream.Seek(0, SeekOrigin.Begin);



                // Create storagecredentials object by reading the values from the configuration (appsettings.json)
                StorageCredentials storageCredentials = new StorageCredentials(AccountName, AccountKey);

                // Create cloudstorage account by passing the storagecredentials
                CloudStorageAccount storageAccount = new CloudStorageAccount(storageCredentials, true);

                // Create the blob client.
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

                // Get reference to the blob container by passing the name by reading the value from the configuration (appsettings.json)
                CloudBlobContainer container = blobClient.GetContainerReference(ImageContainer);
                //container.GetDirectoryReference("dailytrack");
                // Get the reference to the block blob from the container
                string blobPath = $"{folderName}/{fileName}";
                CloudBlockBlob blockBlob = container.GetBlockBlobReference(blobPath);               
                 ContentType = MimeMapping.MimeUtility.GetMimeMapping(fileName);


                blockBlob.Properties.ContentType = ContentType;
                blockBlob.Properties.ContentDisposition = "inline; filename=" + fileName.Split('/').Last(); ;
                // Upload the file
                await blockBlob.UploadFromStreamAsync(memoryStream);

                if (await Task.FromResult(true))
                {
                    //returnFileName = baseURL +"/"+ bucketName + "/" + fileName;
                    returnFileName = fileName;
                }
                else
                {
                    returnFileName = "failed";
                }

                memoryStream.Close();
                data.Close();


                // await Task.FromResult(true);

                return returnFileName;

            }


        }



        public string GetFileURl(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName)) return null;
            fileName = fileName.Trim().Trim('|');
            fileName = fileName.Split('|', System.StringSplitOptions.RemoveEmptyEntries).FirstOrDefault()?.Trim();
            if (string.IsNullOrEmpty(fileName)) return null;
            if (Uri.TryCreate(fileName, UriKind.Absolute, out Uri fileUri))
            {
                if (!Uri.TryCreate(baseURL, UriKind.Absolute, out Uri configuredUri))
                    return fileName;

                string configuredPath = configuredUri.AbsolutePath.TrimEnd('/');
                if (!string.Equals(fileUri.Host, configuredUri.Host, StringComparison.OrdinalIgnoreCase)
                    || !fileUri.AbsolutePath.StartsWith(configuredPath + "/", StringComparison.OrdinalIgnoreCase))
                    return fileName;

                fileName = Uri.UnescapeDataString(fileUri.AbsolutePath.Substring(configuredPath.Length)).TrimStart('/');
            }

            // Create storagecredentials object by reading the values from the configuration (appsettings.json)
            StorageCredentials storageCredentials = new StorageCredentials(AccountName, AccountKey);

            // Create cloudstorage account by passing the storagecredentials
            CloudStorageAccount storageAccount = new CloudStorageAccount(storageCredentials, true);

            // Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Get reference to the blob container by passing the name by reading the value from the configuration (appsettings.json)
            CloudBlobContainer container = blobClient.GetContainerReference(ImageContainer);
            // Blob path must match UploadFileAsync: folderName/fileName
            string normalizedFolderName = folderName?.Trim().Trim('/');
            string normalizedFileName = fileName.TrimStart('/');
            string blobPath = string.IsNullOrEmpty(normalizedFolderName) || normalizedFileName.StartsWith(normalizedFolderName + "/", System.StringComparison.OrdinalIgnoreCase)
                ? normalizedFileName
                : (normalizedFolderName + "/" + normalizedFileName);
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(blobPath);
            SharedAccessBlobPolicy policy = new SharedAccessBlobPolicy()
            {
                Permissions = SharedAccessBlobPermissions.Read,
                SharedAccessExpiryTime = DateTime.UtcNow.AddHours(1)
            };
            string sasToken = blockBlob.GetSharedAccessSignature(policy);
            return blockBlob.Uri.AbsoluteUri + sasToken;
        }

        public void UploadFiles(Stream FileStream, string bucketName, string keyName)
        {


        }

        public string UploadFile(Stream data, string bucketName, string fileName)
        {
            throw new NotImplementedException();

        }


        public void DeleteFile(string fileId, string bucketName)
        {

        }
        public async Task<string> DownloadFile(string filename, string localpath)
        {
            Stream file = null;
            try
            {

                CloudStorageAccount mycloudStorageAccount = CloudStorageAccount.Parse(blobconnectionstring);
                CloudBlobClient blobClient = mycloudStorageAccount.CreateCloudBlobClient();

                CloudBlobContainer container = blobClient.GetContainerReference(ImageContainer);
                CloudBlockBlob cloudBlockBlob = container.GetDirectoryReference(ImageContainer).GetBlockBlobReference(filename);

                // provide the file download location below
                // 
                //localpath = localpath.Replace(filename.Split('/').Last(), "");
                localpath = localpath.Replace(filename, "");
                file = File.Create(localpath + filename.Split('/').Last());



                await cloudBlockBlob.DownloadToStreamAsync(file);
                file.Close();
                file.Dispose();
                return localpath + filename.Split('/').Last();
            }
            catch (Exception ex)
            {
                if (file != null)
                {
                    file.Close();
                    file.Dispose();
                }

                localpath = localpath.Replace(filename, "");
                return localpath + filename.Split('/').Last();
            }

        }


        //public async Task<string> GetFileUrl(string filename, string localpath,string baseurl)
        //{
        //    try
        //    {
        //        CloudStorageAccount mycloudStorageAccount = CloudStorageAccount.Parse(blobconnectionstring);
        //        CloudBlobClient blobClient = mycloudStorageAccount.CreateCloudBlobClient();

        //        CloudBlobContainer container = blobClient.GetContainerReference(ImageContainer);
        //        CloudBlockBlob cloudBlockBlob = container.GetBlockBlobReference(folderName + "/" + filename);

        //        // provide the file download location below            
        //        Stream file = File.Create(localpath + filename.Split('/').Last());



        //        await cloudBlockBlob.DownloadToStreamAsync(file);
        //        file.Close();

        //        return baseurl + "/downloads/" + filename.Split('/').Last();
        //    }catch(Exception ex)
        //    {
        //        Console.WriteLine(ex);
        //        return "exception";
        //    }


        //}
        public List<string> GetImageUrl(string imageFilename)
        {
            List<string> allBlobs = new List<string>();
            try
            { // Create storagecredentials object by reading the values from the configuration (appsettings.json)
                StorageCredentials storageCredentials = new StorageCredentials(AccountName, AccountKey);

                // Create cloudstorage account by passing the storagecredentials
                CloudStorageAccount storageAccount = new CloudStorageAccount(storageCredentials, true);

                // Create the blob client.
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                BlobServiceClient blobServiceClient = new BlobServiceClient(blobconnectionstring);

                // Get reference to the blob container by passing the name by reading the value from the configuration (appsettings.json)
                CloudBlobContainer container = blobClient.GetContainerReference(ImageContainer);
                SharedAccessBlobPolicy policy = new SharedAccessBlobPolicy()
                {
                    Permissions = SharedAccessBlobPermissions.Read,
                    SharedAccessExpiryTime = DateTime.UtcNow.AddHours(1)
                };
                string sasToken = container.GetSharedAccessSignature(policy);
                string[] fileNames = imageFilename?.Split('|');
                foreach (string file in fileNames)
                {
                    if (file != "")
                    {
                        CloudBlockBlob blockBlob = container.GetDirectoryReference(ImageContainer).GetBlockBlobReference(file);

                        allBlobs.Add(blockBlob.Uri.AbsoluteUri + sasToken);
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return allBlobs;
        }

        public string getSASToken()
        {
            string sasToken = null;
            try
            { // Create storagecredentials object by reading the values from the configuration (appsettings.json)
                StorageCredentials storageCredentials = new StorageCredentials(AccountName, AccountKey);

                // Create cloudstorage account by passing the storagecredentials
                CloudStorageAccount storageAccount = new CloudStorageAccount(storageCredentials, true);

                // Create the blob client.
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                BlobServiceClient blobServiceClient = new BlobServiceClient(blobconnectionstring);

                // Get reference to the blob container by passing the name by reading the value from the configuration (appsettings.json)
                CloudBlobContainer container = blobClient.GetContainerReference(ImageContainer);
                SharedAccessBlobPolicy policy = new SharedAccessBlobPolicy()
                {
                    Permissions = SharedAccessBlobPermissions.Read,
                    SharedAccessExpiryTime = DateTime.UtcNow.AddHours(3)
                };
                sasToken = container.GetSharedAccessSignature(policy);
            }
            catch (Exception ex)
            {

            }
            return sasToken;
        }

        public Boolean DeleteFile(string uniqueFileIdentifier)
        {

            // Create storagecredentials object by reading the values from the configuration (appsettings.json)
            StorageCredentials storageCredentials = new StorageCredentials(AccountName, AccountKey);

            // Create cloudstorage account by passing the storagecredentials
            CloudStorageAccount storageAccount = new CloudStorageAccount(storageCredentials, true);

            // Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Get reference to the blob container by passing the name by reading the value from the configuration (appsettings.json)
            CloudBlobContainer container = blobClient.GetContainerReference(ImageContainer);
            //container.GetDirectoryReference("dailytrack");
            // Get the reference to the block blob from the container
            CloudBlockBlob blockBlob = container.GetDirectoryReference(ImageContainer).GetBlockBlobReference(uniqueFileIdentifier.Trim());
           return blockBlob.DeleteIfExistsAsync().GetAwaiter().GetResult();
        }
    }
}
