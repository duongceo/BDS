using HappyRE.Core.Entities.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyRE.Core.Entities.ViewModel
{
    public class SaleOrderListViewModel: BaseEntity
    {
        [Key]
        [ExportIgnore]
        public int Id { get; set; }

        [DisplayName("#")]
        public int RowNumber { get; set; }
        [DisplayName("Mã giao dịch")]
        public string SaleOrderNumber { get; set; }
        [ExportIgnore]
        public int PropertyId { get; set; }

        [DisplayName("Họ tên")]
        public string OwnerName { get; set; }
        [DisplayName("Số điện thoại")]
        public string OwnerPhone { get; set; }
        [DisplayName("Ngày sinh")]
        [ExportIgnore]
        public DateTime? OwnerBirthday { get; set; }
        [DisplayName("CMND/CCCD")]
        public string OwnerIDNumber { get; set; }
        [DisplayName("Địa chỉ")]
        public string OwnerAddress { get; set; }
        [DisplayName("Tiềm năng")]
        [ExportIgnore]
        public int? OwnerTargetId { get; set; }
        [DisplayName("Hình")]
        [ExportIgnore]
        public string OwnerAvatar { get; set; } = "";

        [DisplayName("Họ tên")]
        public string CustomerName { get; set; }
        [DisplayName("Số điện thoại")]
        [ExportIgnore]
        [NonTrack]
        public string CustomerPhone { get; set; }
        [DisplayName("Ngày sinh")]
        [ExportIgnore]
        public DateTime? CustomerBirthday { get; set; }
        [DisplayName("CMND/CCCD")]
        public string CustomerIDNumber { get; set; }
        [DisplayName("Địa chỉ")]
        public string CustomerAddress { get; set; }
        [DisplayName("Hình")]
        [ExportIgnore]
        public string CustomerAvatar { get; set; } = "";
        [DisplayName("Tiềm năng")]
        [ExportIgnore]
        public int? CustomerTargetId { get; set; }
        [DisplayName("Giá trị HĐ/DV")]
        public decimal TotalAmount { get; set; }
        [DisplayName("Điểm thưởng")]
        [ExportIgnore]
        public int RewardPoint { get; set; }
        [DisplayName("Người bán")]
        public string SellBy { get; set; }
        [DisplayName("Người nhập")]
        public string PostedBy { get; set; }
        [DisplayName("Mã BĐS")]
        public string PropertyNumber { get; set; }
        [ExportIgnore]
        public string OwnerTargetHtml { get; set; } = "";
        [ExportIgnore]
        public string CustomerTargetHtml { get; set; } = "";
        [ExportIgnore]
        public string OwnerImageHtml { get; set; }
        [ExportIgnore]
        public string CustomerImageHtml { get; set; }

        [ExportIgnore]
        public string PostedByHtml { get; set; }
        [ExportIgnore]
        public string SellByHtml { get; set; }

        [DisplayName("Ghi chú")]
        [ExportIgnore]
        public string Comment { get; set; }
        [DisplayName("Ngày giao dịch")]
        [ExportIgnore]
        public DateTime? OrderDate { get; set; }
        [DisplayName("Ngày giao dịch")]
        public string OrderDateDisplay => this.OrderDate?.ToString("dd/MM/yyyy");

        [DisplayName("Điểm thưởng")]
        public int RewardPointCalc => (int)Math.Round(this.TotalAmount / 50000000,0);

        [DisplayName("Tiềm năng chủ nhà")]
        public string OwnerTarget => (this.OwnerTargetHtml ?? "").Replace("</br>", "; ");
        [DisplayName("Tiềm năng khách hàng")]
        public string CustomerTarget => (this.CustomerTargetHtml ?? "").Replace("</br>", "; ");

        [ExportIgnore]
        public string PostedByDisplay =>(this.PostedByHtml??"").Replace("</br>", " - ");
        [ExportIgnore]
        public string SellByDisplay => (this.SellByHtml ?? "").Replace("</br>", " - ");
    }
}
