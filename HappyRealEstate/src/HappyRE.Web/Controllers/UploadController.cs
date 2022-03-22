using MBN.Utils;

using HappyRE.Core.Entities;
using HappyRE.Web.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HappyRE.Core.BLL.Repositories;

namespace HappyRE.Web.Controllers
{
    public class UploadController : BaseController
    {
        public UploadController(IUow uow) : base(uow) { }

        #region UploadImage
        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <param name="referCode"></param>
        /// <param name="referId"></param>
        /// <returns></returns>
        public FileUploaderResult UploadImage(FileUploader file, byte referCode = 0, int referId = 0)
        {
            Media objMedia = null;
            bool success = true;
            string msg = "Đăng hình thành công";
            try
            {
                int mediaTypeId = 1; // image
                int profileId = 0;
                objMedia = BLL.Utils.UploadUtils.UploadImage(file, _uow, this.CurrentUserId, mediaTypeId, referCode, referId);
                // avatar
                if (referCode == (int)Core.MediaReferCodeEnum.account_square_logo)
                {
                    profileId = referId = this.UserData.ProfileId;
                    if (profileId > 0 && objMedia != null)
                    {
                        _uow.UserProfile.Mogi_UpdateAvatar(profileId, objMedia);
                    }
                }
                
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                success = false;
            }

            return new FileUploaderResult(success, objMedia, msg);
        }
        #endregion
    }
}