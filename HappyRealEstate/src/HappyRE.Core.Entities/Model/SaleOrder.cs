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
    public class SaleOrder:BaseEntity
    {
        [Key]
        public int Id { get; set; }
        [DisplayName("Bất động sản")]
        public int? PropertyId { get; set; }

        [DisplayName("Họ tên")]
        public string OwnerName { get; set; }
        [DisplayName("Số điện thoại")]
        public string OwnerPhone { get; set; }
        [DisplayName("Ngày sinh")]
        public DateTime? OwnerBirthday { get; set; }
        [DisplayName("CMND/CCCD")]
        public string OwnerIDNumber { get; set; }
        [DisplayName("Địa chỉ")]
        public string OwnerAddress { get; set; }
        [DisplayName("Tiềm năng")]
        public int OwnerTargetId { get; set; }
        [DisplayName("Hình")]
        public string OwnerAvatar { get; set; }

        [DisplayName("Họ tên")]
        public string CustomerName { get; set; }
        [DisplayName("Số điện thoại")]
        public string CustomerPhone { get; set; }
        [DisplayName("Ngày sinh")]
        public DateTime? CustomerBirthday { get; set; }
        [DisplayName("CMND/CCCD")]
        public string CustomerIDNumber { get; set; }
        [DisplayName("Địa chỉ")]
        public string CustomerAddress { get; set; }
        [DisplayName("Hình")]
        public string CustomerAvatar { get; set; }
        public int? CustomerTargetId { get; set; }
        [DisplayName("Giá trị HĐ/DV")]
        public decimal? TotalAmount { get; set; }
        [DisplayName("Địểm thưởng")]
        public int RewardPoint { get; set; }
        [DisplayName("Người bán")]
        public string SellBy { get; set; }
        [DisplayName("Người nhập")]
        public string PostedBy { get; set; }
        [DisplayName("Báo sinh nhật")]
        public bool AlertOwnerBirthDay { get; set; }
        [DisplayName("Báo sinh nhật")]
        public bool AlertCustomerBirthDay { get; set; }
        [DisplayName("Ngày GD")]
        public DateTime? OrderDate { get; set; }
    }
}
