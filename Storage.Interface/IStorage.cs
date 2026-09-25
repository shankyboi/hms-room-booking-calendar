using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Storage.Interface
{
    public interface IStorage
    {
        public string UploadFile(Stream data, string bucketName, string fileName);
        public void DeleteFile(string fileId, string bucketName);

        public string GetFileURl(string fileName);
        public Task<string> UploadFileAsync(Stream data, string bucketName, string fileName);

        public Task<string> DownloadFile(string filename, string localpath);
        public List<string> GetImageUrl(string imageFilename);
        //public Task<string> GetFileUrl(string filename, string localpath, string baseurl);
        public string getSASToken();

        public Boolean DeleteFile(string uniqueFileIdentifier);
    }
}
