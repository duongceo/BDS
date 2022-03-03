using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace HappyRE.Utils
{
    public class GoogleStorageUtil
    {
        private static readonly string root_Image_Folder = "img/";

        public GoogleStorageUtil()
        {

        }
        public void UploadToGooleCloud(string pathToFileName, string contentType, Stream stream)
        {
            GoogleCredential credential = null;
            using (var jsonStream = new FileStream("googleStorageSvc.json", FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                credential = GoogleCredential.FromStream(jsonStream);
            }

            if (pathToFileName.StartsWith("/")) pathToFileName = pathToFileName.Remove(0, 1);
            pathToFileName = root_Image_Folder + pathToFileName;

            using (var storageClient = StorageClient.Create(credential))
            {
                storageClient.UploadObject("cloud.HappyRE.com", pathToFileName, GetMimeType(contentType), stream);
            }
        }

        string GetMimeType(string ext)
        {
            string mimeType = null;
            switch (ext)
            {
                case ".apng":
                    mimeType = "image/apng";
                    break;
                case ".bmp":
                    mimeType = "image/bmp";
                    break;
                case ".gif":
                    mimeType = "image/gif";
                    break;
                case ".ico":
                    mimeType = "image/x-icon";
                    break;
                case ".jpg":
                    mimeType = "image/jpeg";
                    break;
                case ".png":
                    mimeType = "image/png";
                    break;
                case ".svg":
                    mimeType = "image/svg+xml";
                    break;
                case ".webp":
                    mimeType = "image/webp";
                    break;
            }
            return mimeType;
        }
    }
}
