using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HappyRE.Core.Entities.Model
{
    [Serializable]
    public class Property:BaseEntity
    {
        [Key]
        [NonTrack]
        public int Id { get; set; }
        [DisplayName("Mã BĐS")]
        [Required(ErrorMessage = "{0} không được để trống")]       
        public string Code { get; set; }
        [DisplayName("Loại hợp đồng")]
        [Required(ErrorMessage = "{0} không được để trống")]
        public int? ContractId { get; set; }
        [DisplayName("Mã hợp đồng")]
        [Required(ErrorMessage = "{0} không được để trống")]
        public string ContractCode { get; set; }
        [DisplayName("Phí môi giới")]
        [Required(ErrorMessage = "{0} không được để trống")]
        public string Commission { get; set; }
        [DisplayName("Đã check")]
        [Required(ErrorMessage = "{0} không được để trống")]
        public bool IsChecked { get; set; }
        [DisplayName("Trạng thái")]
        [Required(ErrorMessage = "{0} không được để trống")]
        public int StatusId { get; set; }
        [DisplayName("Loại BĐS")]
        [Required(ErrorMessage = "{0} không được để trống")]
        public int TypeId { get; set; }
        [DisplayName("Tỉnh thành")]
        [Required(ErrorMessage = "{0} không được để trống")]
        public int CityId { get; set; }
        [DisplayName("Quận huyện")]
        [Required(ErrorMessage = "{0} không được để trống")]
        public int DistrictId { get; set; }
        [DisplayName("Phường xã")]
        [Required(ErrorMessage = "{0} không được để trống")]
        public int WardId { get; set; }
        [DisplayName("Đường")]
        public int StreetId { get; set; }
        [DisplayName("Số nhà")]
        public string Address { get; set; }
        [DisplayName("Tòa nhà")]
        public string Office { get; set; }
        [DisplayName("Số thửa")]
        public string RegionCode { get; set; }
        [DisplayName("Số tờ bản đồ")]
        public string MapCode { get; set; }
        [DisplayName("Tên chủ nhà")]
        public string OwnerName { get; set; }
        [DisplayName("SĐT chủ nhà")]
        public string OwnerPhone { get; set; }
        [DisplayName("Liên hệ khác")]
        public string OwnerPhoneExt { get; set; }
        [DisplayName("Ghi chú chủ nhà")]
        public string OwnerNote { get; set; }
        [DisplayName("Pháp lý")]
        [Required(ErrorMessage = "{0} không được để trống")]
        public int? LegalId { get; set; }
        [DisplayName("Giá")]
        [Required(ErrorMessage = "{0} không được để trống")]
        public decimal? Price { get; set; }
        [DisplayName("Giá chốt")]
        [Required(ErrorMessage = "{0} không được để trống")]
        public decimal? PriceMatched { get; set; }
        [DisplayName("Đơn vị tiền")]
        [Required(ErrorMessage = "{0} không được để trống")]
        public string CurrencyType { get; set; }
        [DisplayName("Phương thức tính")]
        [Required(ErrorMessage = "{0} không được để trống")]
        public string CalcMethod { get; set; }
        [DisplayName("Chiều rộng")]
        [Required(ErrorMessage = "{0} không được để trống")]
        public double? Width { get; set; }
        [DisplayName("Chiều dài")]
        [Required(ErrorMessage = "{0} không được để trống")]
        public double? Length { get; set; }
        [DisplayName("Tổng diện tích")]
        [Required(ErrorMessage = "{0} không được để trống")]
        public double? Area { get; set; }
        [DisplayName("Diện tích xây dựng")]
        public double? AreaForBuild { get; set; }
        [DisplayName("Số phòng ngủ")]
        public int? NumOfBedroom { get; set; }
        [DisplayName("Số toilet")]
        public int? NumOfToilet { get; set; }
        [DisplayName("Số tầng")]
        public int? NumOfFloor { get; set; }
        [DisplayName("Đường rộng")]
        public double? StreetWidth { get; set; }
        [DisplayName("Hướng")]
        public int? DirectionId { get; set; }
        [DisplayName("Tiện ích")]
        public int UtilityId { get; set; }
        [DisplayName("Nguồn")]
        public int? SourceId { get; set; }
        [DisplayName("Ghi chú")]
        public string Note { get; set; }
        [DisplayName("BĐS tốt")]
        public bool IsGood { get; set; }
        [DisplayName("HOT")]
        public bool IsHot { get; set; }
        [DisplayName("Xác thực")]
        public bool IsVerified { get; set; }
        [DisplayName("Đặc điểm tốt")]
        public int StrongId { get; set; }
        [DisplayName("Đặc điểm xấu")]
        public int WeakId { get; set; }
        [DisplayName("Mức độ xây dựng")]
        public int ContructId { get; set; }
        [DisplayName("Kết cấu xây dựng")]
        public int StructureId { get; set; }
        [DisplayName("Tiềm năng")]
        public int PotentialId { get; set; }
        [DisplayName("Hình")]
        [NonTrack]
        public string ImageUrl { get; set; }
        [DisplayName("Link video")]
        public string VideoUrl { get; set; }
        [DisplayName("Người nhập")]
        public string PostedBy { get; set; }
        [DisplayName("Ngày nhập")]
        [NonTrack]
        public DateTime PostedDate { get; set; }
        [NonTrack]
        public int Sort { get; set; }
        [NonTrack]
        public bool IsForceHiddenPhone { get; set; }
        [NonTrack]
        public bool IsTemp { get; set; }

        [NotMapped]
        [NonTrack]
        public List<string> PropertyImages { get; set; } = new List<string>();
        [NotMapped]
        [NonTrack]
        public bool CanSaveTemp => this.Id == 0 || this.IsTemp == true;
    }
}
