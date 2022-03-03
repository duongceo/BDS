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
    public class PropertyListViewModel: BaseEntity
    {
        [Key]
        [ExportIgnore]
        public int Id { get; set; }

        [DisplayName("#")]
        public int RowNumber { get; set; }
        [DisplayName("Mã tin đăng")]
        public string PropertyNumber { get; set; }

        [DisplayName("Mã BĐS")]
        [Required(ErrorMessage = "{0} không được để trống")]
        public string Code { get; set; }
        [DisplayName("Loại hợp đồng")]
        [Required(ErrorMessage = "{0} không được để trống")]
        [ExportIgnore]
        public int ContractId { get; set; }
        [DisplayName("Mã hóa đơn")]
        [Required(ErrorMessage = "{0} không được để trống")]
        public string ContractCode { get; set; }
        [DisplayName("Phí môi giới")]
        [Required(ErrorMessage = "{0} không được để trống")]
        public string Commission { get; set; }
        [DisplayName("Đã check")]
        [Required(ErrorMessage = "{0} không được để trống")]
        [ExportIgnore]
        public bool IsChecked { get; set; }
        [DisplayName("Trạng thái")]
        [Required(ErrorMessage = "{0} không được để trống")]
        [ExportIgnore]
        public int StatusId { get; set; }
        [DisplayName("Loại BĐS")]
        [Required(ErrorMessage = "{0} không được để trống")]
        [ExportIgnore]
        public int TypeId { get; set; }
        [DisplayName("Tỉnh thành")]
        [Required(ErrorMessage = "{0} không được để trống")]
        [ExportIgnore]
        public int CityId { get; set; }
        [DisplayName("Quận huyện")]
        [Required(ErrorMessage = "{0} không được để trống")]
        public int DistrictId { get; set; }
        [DisplayName("Phường xã")]
        [Required(ErrorMessage = "{0} không được để trống")]
        [ExportIgnore]
        public int WardId { get; set; }
        [DisplayName("Đường")]
        [ExportIgnore]
        public int? StreetId { get; set; }
        [DisplayName("Địa chỉ")]
        [ExportIgnore]
        public string Address { get; set; }
        [DisplayName("Tòa nhà")]
        public string Office { get; set; }
        [DisplayName("Số thửa")]
        public string RegionCode { get; set; }
        [DisplayName("Số bản đồ")]
        public string MapCode { get; set; }
        [DisplayName("Tên chủ nhà")]
        public string OwnerName { get; set; }
        [DisplayName("Sđt chủ nhà")]
        public string OwnerPhone { get; set; }
        [DisplayName("Liên hệ khác")]
        public string OwnerPhoneExt { get; set; }
        [DisplayName("Ghi chú chủ nhà")]
        public string OwnerNote { get; set; }
        [DisplayName("Pháp lý")]
        [Required(ErrorMessage = "{0} không được để trống")]
        [ExportIgnore]
        public int LegalId { get; set; }
        [DisplayName("Giá bán/thuê")]
        [Required(ErrorMessage = "{0} không được để trống")]
        [ExportIgnore]
        public decimal Price { get; set; }
        [ExportIgnore]
        public decimal PriceVND { get; set; }

        [DisplayName("Đơn giá")]
        [Required(ErrorMessage = "{0} không được để trống")]
        public string PricePerArea => this.Area>0? (this.PriceVND / (decimal)this.Area).ToString("N0"):"";

        [DisplayName("Giá chốt")]
        [Required(ErrorMessage = "{0} không được để trống")]
        [ExportIgnore]
        public decimal PriceMatched { get; set; }
        [DisplayName("Đơn vị tiền")]
        [Required(ErrorMessage = "{0} không được để trống")]
        public string CurrencyType { get; set; }
        [DisplayName("Phương thức tính")]
        [Required(ErrorMessage = "{0} không được để trống")]
        public string CalcMethod { get; set; }
        [DisplayName("Chiều rộng")]
        [Required(ErrorMessage = "{0} không được để trống")]
        public double Width { get; set; }
        [DisplayName("Chiều dài")]
        [Required(ErrorMessage = "{0} không được để trống")]
        public double Length { get; set; }
        [DisplayName("Tổng diện tích")]
        [Required(ErrorMessage = "{0} không được để trống")]
        public double Area { get; set; }
        [DisplayName("Diện tích xây dựng")]
        public double? AreaForBuild { get; set; }
        [DisplayName("Số phòng ngủ")]
        public int NumOfBedroom { get; set; }
        [DisplayName("Số toilet")]
        public int NumOfToilet { get; set; }
        [DisplayName("Số tầng")]
        public int NumOfFloor { get; set; }
        [DisplayName("Đường rộng")]
        public double StreetWidth { get; set; }
        [DisplayName("Hướng")]
        [ExportIgnore]
        public int DirectionId { get; set; }
        [DisplayName("Tiện ích")]
        [ExportIgnore]
        public int UtilityId { get; set; }
        [DisplayName("Nguồn")]
        [ExportIgnore]
        public int SourceId { get; set; }
        [DisplayName("Ghi chú")]
        public string Note { get; set; }
        [DisplayName("BĐS tốt")]
        public bool IsHot { get; set; }
        [DisplayName("Xác thực")]
        public bool IsVerified { get; set; }
        [DisplayName("Đặc điểm tốt")]
        public int StrongId { get; set; }
        [DisplayName("Đặc điểm xấu")]
        [ExportIgnore]
        public int WeakId { get; set; }
        [DisplayName("Mức độ xây dựng")]
        [ExportIgnore]
        public int? ContructId { get; set; }
        [DisplayName("Kết cấu xây dựng")]
        [ExportIgnore]
        public int? StructureId { get; set; }
        [DisplayName("Tiềm năng")]
        [ExportIgnore]
        public int? PotentialId { get; set; }
        [DisplayName("Hình")]
        [ExportIgnore]
        public string ImageUrl { get; set; }
        [DisplayName("Link video")]
        [ExportIgnore]
        public string VideoUrl { get; set; }
        [DisplayName("Người đăng")]
        public string PostedBy { get; set; }
        [DisplayName("Ngày đăng")]
        [ExportIgnore]
        public DateTime PostedDate { get; set; }
        [ExportIgnore]
        public bool IsForceHiddenPhone { get; set; }
        [ExportIgnore]
        public int ViewedMobileToday { get; set; }
        [ExportIgnore]
        public bool IsViewedMobileToday => this.ViewedMobileToday > 0;

        [DisplayName("Trạng thái")]
        [ExportIgnore]
        public string StatusHtml { get; set; } = "";
        [DisplayName("Loại Bđs")]
        [ExportIgnore]
        public string TypeHtml { get; set; } = "";
        [DisplayName("Tiềm năng")]
        [ExportIgnore]
        public string PotentialHtml { get; set; } = "";
        [DisplayName("Địa chỉ")]
        [ExportIgnore]
        public string AddressHtml { get; set; } = "";
        [DisplayName("Loại hợp đồng")]
        public string ContractName { get; set; }
        [DisplayName("Nguồn")]
        public string SourceName { get; set; }
        [DisplayName("Hướng nhà")]
        public string DirectionName { get; set; }
        [DisplayName("Pháp lý")]
        public string LegalName { get; set; }
        [DisplayName("Thông tin chi tiết")]
        [ExportIgnore]
        public string DetailHtml { get; set; } = "";
        [DisplayName("Người nhập")]
        [ExportIgnore]
        public string PostedHtml { get; set; } = "";
        [ExportIgnore]
        public string ImageHtml { get; set; } = "";
        [DisplayName("Đặc điểm tốt")]
        [ExportIgnore]
        public string StrongHtml { get; set; } = "";
        [DisplayName("Đặc điểm xấu")]
        [ExportIgnore]
        public string WeakHtml { get; set; } = "";
        [DisplayName("Mức độ xây dựng")]
        [ExportIgnore]
        public string ContructHtml { get; set; } = "";
        [DisplayName("Kết cấu CTXD")]
        [ExportIgnore]
        public string StructureHtml { get; set; } = "";
        [DisplayName("Tiện ích")]
        [ExportIgnore]
        public string UtilityHtml { get; set; } = "";
        [ExportIgnore]
        public List<string> Images { get; set; }
        [DisplayName("Bình luận")]
        [ExportIgnore]
        public string Comment { get; set; }

        [DisplayName("Giá")]
        public string PriceDisplay => this.Price==0?"": $"{this.Price.ToString("N0")} {this.CurrencyType}/{this.CalcMethod}";
        [DisplayName("Giá chốt")]
        public string PriceMatchedDisplay => this.PriceMatched==0?"": $"{this.PriceMatched.ToString("N0")} {this.CurrencyType}/{this.CalcMethod}";
        [DisplayName("Trạng thái")]
        public string Status => (this.StatusHtml ?? "").Replace("</br>", "; ");
        [DisplayName("Loại BĐS")]
        public string Type => (this.TypeHtml ?? "").Replace("</br>", "; ");
        [DisplayName("Tiềm năng")]
        public string Potential => (this.PotentialHtml ?? "").Replace("</br>", "; ");
        [DisplayName("Vị trí")]
        public string Location => (this.AddressHtml ?? "").Replace("</br>", ". ").Replace("<b>", "").Replace("</b>", "");
        [DisplayName("Hướng")]
        public string Direction => (this.DirectionName??"").Replace("</br>", "; ");
        [DisplayName("Nội dung")]
        public string Detail => (this.DetailHtml ?? "").Replace("</br>", ". ").Replace("<b>", "").Replace("</b>", "");
        [ExportIgnore]
        public string Posted => (this.PostedHtml ?? "").Replace("</br>", " - ");
        [DisplayName("Điểm mạnh")]
        public string Strong => (this.StrongHtml ?? "").Replace("</br>", "; ");
        [DisplayName("Điểm yếu")]
        public string Weak => (this.WeakHtml ?? "").Replace("</br>", "; ");
        [DisplayName("Tiện ích")]
        public string Utility => (this.UtilityHtml ?? "").Replace("</br>", "; ");
        [DisplayName("Kết cấu XD")]
        public string Contruct => (this.ContructHtml ?? "").Replace("</br>", "; ");
        [DisplayName("Kiến trúc")]
        public string Structure => (this.StructureHtml ?? "").Replace("</br>", "; ");
        [DisplayName("Nguồn")]
        public string Source => (this.SourceName ?? "").Replace("</br>", "; ");
    }
}
