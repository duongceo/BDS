using HappyRE.Core.Entities.Model;
using MBN.Utils.Extension;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyRE.Core.Entities.ViewModel
{
    public class ClaimData
    {
        public int ProfileId { get; set; }
        public string UserId { get; set; }
        public int CustomerId { get; set; }
        public string UserName { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Avatar { get; set; }
        public bool IsVerifiedMobile { get; set; }
        public bool IsVerifiedEmail { get; set; }
        public bool IsVerifiedIDCard { get; set; }
        public bool IsAutoApproved { get { return this.IsVerifiedIDCard; } }

        public int LevelId { get; set; }

        public Guid? OwnerBy { get; set; }

        public DateTime Created = DateTime.Now;
        public bool RememberMe { get; set; }
        public string Roles { get; set; } = string.Empty;

        public DateTime LastChecked = DateTime.Now;
        public bool IsExpired(int minutes = 15)
        {
            return (LastChecked.AddMinutes(minutes) < DateTime.Now);
        }
        public bool IsNeedVerifiedMobile()
        {
            return (string.IsNullOrEmpty(this.Mobile) == false && this.IsVerifiedMobile == false);
        }

        public string Small()
        {
            return (new
            {
                ProfileId,
                Mobile,
                Email,
                FullName,
                LevelId,
                IsVerifiedMobile,
                IsVerifiedEmail,
                IsVerifiedIDCard,
                Avatar
            }).ToJson();
        }

        public static ClaimData MapObject(UserProfile profile, bool rememberMe = true)
        {
            return new ClaimData()
            {
                UserId = profile.UserId,
                UserName = profile.UserName,
                ProfileId = profile.Id,
                Mobile = profile.Mobile,
                Email = profile.Email,
                FullName = profile.FullName,
                Avatar = profile.Avatar,
                IsVerifiedMobile = profile.IsVerifyMobile,
                IsVerifiedEmail = profile.IsVerifyEmail,
                LevelId = profile.LevelId??0,
                RememberMe = rememberMe,
                //Roles = profile.GetRole(),
                LastChecked = DateTime.Now
            };
        }
    }

    public class GoogleCaptchaResponse
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("challenge_ts")]
        public DateTime Time { get; set; }

        [JsonProperty("hostname")]
        public string HostName { get; set; }

        [JsonProperty("error-codes")]
        public List<string> ErrorCodes { get; set; }

        public string GetError()
        {
            string res = string.Empty;
            if (res == "missing-input-secret")
            {
                res = "The secret parameter is missing.";
            }
            else if (res == "invalid-input-secret")
            {
                res = "The secret parameter is invalid or malformed.";
            }
            else if (res == "missing-input-response")
            {
                res = "The response parameter is missing.";
            }
            else if (res == "invalid-input-response")
            {
                res = "The response parameter is invalid or malformed.";
            }
            else if (res == "timeout-or-duplicate")
            {
                res = "The response is timeout.";
            }
            return res;
        }
    }

    [Serializable]
    public class AjaxResponse
    {
        /// <summary>
        /// Dữ liệu trả về
        /// </summary>
        public object Data { get; set; }

        /// <summary>
        /// Trạng thái
        /// </summary>
        public bool Status { get; set; }

        /// <summary>
        /// Thông báo
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Mã lỗi nếu có
        /// </summary>
        public string ErrorCode { get; set; }
    }
}
