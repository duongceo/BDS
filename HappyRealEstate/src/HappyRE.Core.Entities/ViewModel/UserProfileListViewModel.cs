using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyRE.Core.Entities.ViewModel
{
    public class UserProfileListViewModel
    {
        [Key]
        [ExportIgnore]
        public int Id { get; set; }
        [DisplayName("#")]
        public int RowNumber { get; set; }
        [ExportIgnore]
        public string UserId { get; set; }
        [DisplayName("Tên đăng nhập")]
        public string UserName { get; set; }
        public string Email { get; set; }
        [DisplayName("Họ tên")]
        public string FullName { get; set; }
        [DisplayName("Số điện thoại")]
        public string Mobile { get; set; }
        [DisplayName("Hình ảnh")]
        public string Avatar { get; set; }
        [DisplayName("Chức vụ")]
        public string LevelName { get; set; }
        [DisplayName("Phòng ban")]
        public string DepartmentName { get; set; }
        [DisplayName("Nhóm quyền")]
        public string RoleGroupName { get; set; }
        public int UserType { get; set; }
        [DisplayName("Trạng thái")]
        public int UserStatus { get; set; }
        [DisplayName("Ngày sinh")]
        [ExportIgnore]
        public DateTime? Birthday { get; set; }
        [DisplayName("Ngày sinh")]
        public string BirthdayDisplay => this.Birthday?.ToString("dd/MM/yyyy");
        [ExportIgnore]
        public DateTime ActiveDate { get; set; }

        [ExportIgnore]
        public string NameDisplay => this.FullName +  $" ({this.UserName})";
    }

    public class UserDepartmentModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Type { get; set; }
    }

}
