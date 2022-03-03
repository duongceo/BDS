using HappyRE.Core.Entities.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HappyRE.App.Models
{
    public class ClaimData
    {
        public int ProfileId { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Avatar { get; set; }
        public bool IsVerifiedMobile { get; set; }
        public bool IsVerifiedEmail { get; set; }
        public int? LevelId { get; set; }
        public int? DepartmentId { get; set; }
        public int? RoleGroupId { get; set; }

        public DateTime Created = DateTime.Now;
        public bool RememberMe { get; set; }

        public DateTime LastChecked = DateTime.Now;
        public bool IsExpired(int minutes = 15)
        {
            return (LastChecked.AddMinutes(minutes) < DateTime.Now);
        }
        public bool IsNeedVerifiedMobile()
        {
            return (string.IsNullOrEmpty(this.Mobile) == false && this.IsVerifiedMobile == false);
        }

        //public string Small()
        //{
        //    return (new
        //    {
        //        ProfileId,
        //        Mobile,
        //        Email,
        //        FullName,
        //        LevelId,
        //        DepartmentId,
        //        RoleGroupId,
        //        IsVerifiedMobile,
        //        IsVerifiedEmail,
        //        IsVerifiedIDCard,
        //        Avatar
        //    }).ToJson();
        //}

        public static ClaimData MapObject(UserProfile profile, bool rememberMe = true)
        {
            return new ClaimData()
            {
                ProfileId = profile.Id,
                UserId = profile.UserId,
                UserName = profile.UserName,
                Mobile = profile.Mobile,
                Email = profile.Email,
                FullName = profile.FullName,
                Avatar = profile.Avatar,
                IsVerifiedMobile = profile.IsVerifyMobile,
                IsVerifiedEmail = profile.IsVerifyEmail,
                LevelId = profile.LevelId,
                DepartmentId = profile.DepartmentId,
                RoleGroupId= profile.RoleGroupId,
                RememberMe = rememberMe,
                LastChecked = DateTime.Now
            };
        }

        #region GA
        public string GA_UserId()
        {
            return " dataLayer = [{'userID': 'agent." + this.ProfileId + (string.IsNullOrEmpty(this.Mobile) == true ? "" : ".m" + this.Mobile) + "'}];";
        }
        #endregion
    }
}