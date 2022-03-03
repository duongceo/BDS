using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyRE.Core.Entities.Model
{
    [Serializable]
    public class UserProfile:BaseEntity
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; }
        [DisplayName("Tên đăng nhập")]
        public string UserName { get; set; }
        public string Email { get; set; }
        [DisplayName("Họ tên")]
        public string FullName { get; set; }
        [DisplayName("Số điện thoại")]
        public string Mobile { get; set; }
        public string Avatar { get; set; }
        [DisplayName("Chức vụ")]
        public int? LevelId { get; set; }
        [DisplayName("Phòng ban")]
        public int? DepartmentId { get; set; }
        [DisplayName("Nhóm quyền")]
        public int? RoleGroupId { get; set; } = 0;
        public int UserType { get; set; }
        [DisplayName("Trạng thái")]
        public int UserStatus { get; set; }
        public DateTime? Birthday { get; set; }        
        public DateTime ActiveDate { get; set; }        
        public bool IsVerifyMobile { get; set; }
        public bool IsVerifyEmail { get; set; }
        public string Provider { get; set; }
        public string ProviderId { get; set; }
        
    }
}
