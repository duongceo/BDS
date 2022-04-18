using HappyRE.Core.Utils;
using HappyRE.Core.Entities;
using HappyRE.Core.Entities.Model;
using HappyRE.Core.Entities.ViewModel;
using HappyRE.FileServiceProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Hangfire;

namespace HappyRE.Core.BLL.Repositories
{
    public class FileRepository : BaseRepository<File>, IFileRepository
    {
        private static readonly int FILE_MAXSIZE = int.Parse(ConfigSettings.Get("FILE_MAXSIZE", "5"));
        private static readonly string FILE_IMG_ALLOW = ConfigSettings.Get("FILE_IMG_ALLOW", "'.jpg,image/*,.jpeg,.png,.gif,.svg,.ico'");
        private static readonly string FILE_API_DOMAIN = "https://static.lenmay.vn";
        public FileRepository(IUow uow) : base(uow) { }

        public void AddFile(File file)
        {          
            this.Add(file);
            this.Commit();
        }

        public void Delete(List<Guid> ids,string userId)
        {
            foreach(var id in ids){
                var file = _dbSet.Find(id);
                if (file != null)
                {
                    if (IsOwnerOrAdmin(file.UserId, userId) == false) return;
                    uow.FileService.RemoveFile(file.UserId, file.Name);
                    _dbSet.Remove(file);
                    this.Commit();
                }
            }
            //this.RemoveCache(id);
        }

        #region Upload File
        public async Task<FileResponseModel> UploadImg(System.IO.Stream file, string fileName, int type,string collection, int? refId=0, string groupCode="")
        {
            if (string.IsNullOrEmpty(fileName) == false) fileName = fileName.ToLower();
            if ((file.Length / (1024 * 1024)) > FILE_MAXSIZE)
            {
                throw new BusinessException(Entities.Resources.Messages.Image_Err_MaxSize);
            }
            var ext = System.IO.Path.GetExtension(fileName);
            if (!FILE_IMG_ALLOW.Contains(ext))
            {
                throw new BusinessException(Entities.Resources.Messages.Image_Err_Ext);
            }

            using (file)
            {
                var path = MakeFileUrl(fileName, type, collection);
                //var rp = uow.FileService.UploadFile(file, fileName, userId);
                var rp = uow.FileService.UploadFile(file, path);
                if (rp.IsError)
                {
                    throw new BusinessException(rp.Message);
                }
                else
                {
                    if (type == (int)FileType.Property)
                    {
                        if (refId.HasValue && refId > 0 && 1==0)
                        {
                            await uow.ImageFile.IU(new ImageFile()
                            {
                                TableName= "Property",
                                TableKeyId= refId.Value,
                                Src= rp.Data.Thumb, 
                                IsMore=true,
                                GroupCode=groupCode
                            });
                        }
                    }
                    else if (type == (int)FileType.Avatar)
                    {
                        //uow.UserProfile.UpdateAvatar(userId,rp.Data.path.Replace(FILE_API_DOMAIN, ""));
                    }
                    else if (type == (int)FileType.Blog)
                    {

                    }

                    return rp.Data;
                }
            }
        }


        public string UploadThumFromBase64(string base64String,string userId)
        {
            if (!string.IsNullOrEmpty(base64String))
            {
                byte[] imageBytes = Convert.FromBase64String(base64String.Replace("data:image/jpeg;base64,", ""));
                System.IO.MemoryStream file = new System.IO.MemoryStream(imageBytes);
                using (file)
                {
                    var rp = uow.FileService.UploadFile(file, "thumb" + DateTime.Now.GetHashCode().ToString("x") + ".jpg", userId);
                    if (rp.IsError)
                    {
                        throw new BusinessException(rp.Message);
                    }
                    else
                    {
                        return rp.Data.path.Replace(FILE_API_DOMAIN, "");
                    }
                }
            }
            return null;
        }

        public FileResponseModel UploadImgFromUrl(string url,Guid? coid,string userId)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                throw new BusinessException("Url hình ảnh không hợp lệ");
            }
            if (url.StartsWith(FILE_API_DOMAIN))
            {
                throw new BusinessException("Bạn có thể sử dụng trực tiếp link {url}");
            }
            string pattern = @"(http(s?):)([/|.|\w|\s|-])*\.(?:jpg|gif|png|svg)";
            Regex rgx = new Regex(pattern, RegexOptions.IgnoreCase);
            var ok = rgx.IsMatch(url);
            if (ok)
            {
                var rp = uow.FileService.UploadFileFromUrl(url, userId);
                if (rp.IsError)
                {
                    throw new BusinessException(rp.Message);
                }
                else
                {
                    var id = Guid.NewGuid();
                    this.AddFile(new File()
                    {
                        Id = id,
                        CollectionId= coid,
                        UserId = userId,
                        NameRaw = System.IO.Path.GetFileName(url),
                        Name = System.IO.Path.GetFileName(rp.Data.path),
                        Size = 1,
                        Type = (int)FileType.Property,
                        Path = rp.Data.path.Replace(FILE_API_DOMAIN, "")
                    });
                    rp.Data.id = id;
                    rp.Data.file_s3_size =1;

                    return rp.Data;
                }
            }
            else
            {
                throw new BusinessException("Url hình ảnh không hợp lệ");
            }
        }
        #endregion

        #region Upload File S3
        public async Task<FileResponseModel> UploadImgToS3(System.IO.Stream file, string fileName, int type, Guid? coid, string userId)
        {
            await uow.FileService.UploadFileToS3(file, fileName, userId);
            return null;
        }
        #endregion

        #region utils
        string MakeFileUrl(string fileName, int type, string collection)
        {
            string path = "", typeName="";
            fileName = fileName.Replace(" ", "_");
            switch (type)
            {
                case (int)FileType.Property:
                    typeName = "product";
                    path += string.Join("/", typeName, collection, fileName);
                    break;
                case (int)FileType.Customer:
                    typeName = "customer";
                    path += string.Join("/", typeName, collection, fileName);
                    break;
                case (int)FileType.Avatar:
                    typeName = "avatar";
                    path += string.Join("/", typeName, fileName);
                    break;
                case (int)FileType.Blog:
                    typeName = "blog";
                    path += string.Join("/", typeName, collection, fileName);
                    break;
            }
            return path;
        }
        #endregion

    }
}
