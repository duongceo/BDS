using HappyRE.Core.Entities.Model;
using HappyRE.Core.Entities.ViewModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HappyRE.Core.BLL.Repositories
{
    public interface IFileRepository: IBaseRepository<File>
    {
        FileResponseModel UploadImg(System.IO.Stream file, string fileName, int type, string collection);
        FileResponseModel UploadImgFromUrl(string url, Guid? coid, string userId);
        string UploadThumFromBase64(string base64String, string userId);
        void Delete(List<Guid> id, string userId);
        Task<FileResponseModel> UploadImgToS3(System.IO.Stream file, string fileName, int type, Guid? coid, string userId);
    }
}