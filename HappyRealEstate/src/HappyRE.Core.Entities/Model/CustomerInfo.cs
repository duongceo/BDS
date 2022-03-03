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
    public class CustomerInfo:BaseEntity
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("Họ tên")]
        public string FullName { get; set; }
        [DisplayName("Số điện thoại")]
        [Required(ErrorMessage = "{0} không được để trống")]
        public string Phone { get; set; }

        [DisplayName("Ngày sinh")]
        public DateTime? Birthday { get; set; }
        [DisplayName("Hình")]
        public string Avatar { get; set; }
        [DisplayName("CMND")]
        public string IdCardNumber { get; set; }
        [DisplayName("Địa chỉ")]
        public string Address { get; set; }
        [DisplayName("Ghi chú")]
        public string Note { get; set; }
        [DisplayName("Báo sinh nhật")]
        public bool AlertBirthDay { get; set; }
    }
}
