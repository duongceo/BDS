using HappyRE.Core.Entities.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyRE.Core.Entities.Model
{
    public class NonTrack : Attribute { }
}

namespace HappyRE.Core.Entities.ViewModel
{
    public class ExportIgnore : Attribute { }
    
    public class CustomerListViewModel : BaseEntity
    {
        [Key]
        [ExportIgnore]
        public int Id { get; set; }

        [DisplayName("#")]
        public int RowNumber {get;set;}

        [DisplayName("Họ tên")]
        [Description("First name")]
        public string FullName { get; set; }
        [DisplayName("Số điện thoại")]
        public string Phone { get; set; }
        [DisplayName("Số điện thoại khác")]
        public string ExtPhone { get; set; }
        [DisplayName("Ngày sinh")]
        public string BirthdayDisplay => this.Birthday.HasValue ? this.Birthday.Value.ToString("dd/MM/yyyy") : "";
        [ExportIgnore]
        [DisplayName("Ngày sinh")]
        public DateTime? Birthday { get; set; }
        [DisplayName("Hình")]
        [ExportIgnore]
        public string Avatar { get; set; }
        [DisplayName("Phân loại")]
        public string ContractName { get; set; }
        [DisplayName("Trạng thái")]
        [ExportIgnore]
        public string StatusHtml { get; set; } = "";
        [DisplayName("Nhu cầu")]
        [ExportIgnore]
        public string DemandHtml { get; set; } = "";
        [DisplayName("Mục đích")]
        [ExportIgnore]
        public string TargetHtml { get; set; } = "";
        [DisplayName("Khu vực")]
        [ExportIgnore]
        public string RegionTargetHtml { get; set; } = "";
        [DisplayName("Nguồn khách")]
        public string SourceName { get; set; }
        [DisplayName("Hướng nhà")]
        [ExportIgnore]
        public string DirectionHtml { get; set; } = "";
        [DisplayName("Thông tin chi tiết")]
        [ExportIgnore]
        public string DetailHtml { get; set; } = "";
        [DisplayName("Người nhập")]
        [ExportIgnore]
        public string CreatedByHtml { get; set; } = "";
        [ExportIgnore]
        public string ImageHtml { get; set; } = "";

        [DisplayName("Tài chính")]
        public decimal BudgetFrom { get; set; }
        [DisplayName("đến")]
        public decimal BudgetTo { get; set; }
        [DisplayName("Diện tích tối thiểu")]
        public decimal MinArea { get; set; }
        [DisplayName("Chiều rộng tối thiểu")]
        public decimal MinWidth { get; set; }
        [DisplayName("Chiều dài tối thiểu")]
        public decimal MinLength { get; set; }
        [DisplayName("Đường rộng tối thiểu")]
        public float StreetWidth { get; set; }
        [DisplayName("Đơn vị tiền")]
        [ExportIgnore]
        public string CurrencyType { get; set; }
        [DisplayName("Phương thức")]
        [ExportIgnore]
        public string CalcMethod { get; set; }
        [DisplayName("Số tầng")]
        public string NumOfFloor { get; set; }
        [DisplayName("Số phòng")]
        public string NumOfRoom { get; set; }
        [DisplayName("Ghi chú")]
        public string Note { get; set; }
        [ExportIgnore]
        public int ViewedMobileToday { get; set; }
        [ExportIgnore]
        public bool IsForceHiddenPhone { get; set; }
        [ExportIgnore]
        public bool IsViewedMobileToday => this.ViewedMobileToday > 0;
        [DisplayName("Trạng thái")]
        public string Status => this.StatusHtml.Replace("</br>", ". ");
        [DisplayName("Nhu cầu")]
        public string Demand => this.DemandHtml.Replace("</br>", ". ");
        [DisplayName("Mục đích")]
        public string Target => this.TargetHtml.Replace("</br>", ". ");
        [DisplayName("Hướng")]
        public string Direction => this.DirectionHtml.Replace("</br>", ". ");
        [DisplayName("Nội dung")]
        public string Detail => this.DetailHtml.Replace("</br>", ". ").Replace("<b>","").Replace("</b>","");
        [DisplayName("Người nhập")]
        public string PostedBy => this.CreatedByHtml.Replace("</br>", " - ");
        [DisplayName("Khu vực")]
        [ExportIgnore]
        public string RegionTargetDisplay => Utils.StringUtils.RemoveComma(this.RegionTargetHtml);
        [DisplayName("Khu vực")]
        public string Region => this.RegionTargetDisplay.Replace("</br>", ". ").Replace("<b>", "").Replace("</b>", "");
    }
}
