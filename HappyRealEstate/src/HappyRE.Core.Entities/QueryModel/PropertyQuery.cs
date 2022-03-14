using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyRE.Core.Entities
{
    public class PropertyQuery: BaseQuery
    {
        [DisplayName("Mã BĐS")]
        public string Code { get; set; }
        [DisplayName("Loại BĐS")]
        public int? TypeId_Filter { get; set; }
        [DisplayName("Loại hợp đồng")]
        public int? ContractId_Filter { get; set; }
        [DisplayName("Mã hóa đơn")]
        public string ContractCode { get; set; }
        [DisplayName("Phí môi giới")]
        public string Commission { get; set; }
        [DisplayName("Đã check")]
        public bool? IsChecked_Filter { get; set; }
        [DisplayName("Trạng thái")]
        public int? StatusId_Filter { get; set; }
        [DisplayName("Loại BĐS")]
        public int? TypeId { get; set; }
        [DisplayName("Tỉnh thành")]
        public int? CityId { get; set; }
        [DisplayName("Quận huyện")]
        public int? DistrictId { get; set; }
        [DisplayName("Phường xã")]
        public int? WardId { get; set; }
        [DisplayName("Đường")]
        public int? StreetId { get; set; }
        [DisplayName("Địa chỉ")]
        public string Address { get; set; }
        [DisplayName("Tòa nhà")]
        public string Office { get; set; }
        [DisplayName("Số thửa")]
        public string RegionCode { get; set; }
        [DisplayName("Số bản đồ")]
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
        public int? LegalId_Filter { get; set; }
        [DisplayName("Giá chào bán")]
        public string Price_bw { get; set; }
        [DisplayName("Giá chốt")]
        public decimal PriceMatched { get; set; }
        [DisplayName("Đơn vị tiền")]
        public string CurrrencyType { get; set; }
        [DisplayName("Phương thức tính")]
        public string CalcMethod { get; set; }
        [DisplayName("Chiều rộng")]
        public string Width_bw { get; set; }
        [DisplayName("Chiều dài")]
        public string Length_bw { get; set; }
        [DisplayName("Tổng diện tích")]
        public string Area_bw { get; set; }
        [DisplayName("Diện tích xây dựng")]
        public string AreaForBuild_bw { get; set; }
        [DisplayName("Số phòng ngủ")]
        public string NumOfBedroom_bw { get; set; }
        [DisplayName("Số toilet")]
        public string NumOfToilet_bw { get; set; }
        [DisplayName("Số tầng")]
        public string NumOfFloor_bw { get; set; }
        [DisplayName("Đường rộng")]
        public string StreetWidth_bw { get; set; }
        [DisplayName("Hướng")]
        public int? DirectionId_Filter { get; set; }
        [DisplayName("Tiện ích")]
        public int? UtilityId_Filter { get; set; }
        [DisplayName("Nguồn")]
        public int? SourceId_Filter { get; set; }
        [DisplayName("Ghi chú")]
        public string Note { get; set; }
        [DisplayName("BĐS tốt")]
        public bool IsHot { get; set; }
        [DisplayName("Đặc điểm tốt")]
        public int? StrongId { get; set; }
        [DisplayName("Đặc điểm xấu")]
        public int? WeakId { get; set; }
        [DisplayName("Mức độ xây dựng")]
        public int? ContructId { get; set; }
        [DisplayName("Kết cấu xây dựng")]
        public int? StructureId { get; set; }
        [DisplayName("Tiềm năng")]
        public int? PotentialId { get; set; }
        [DisplayName("Người đăng")]
        public string PostedBy { get; set; }
        [DisplayName("Ngày đăng")]
        public DateTime PostedDate { get; set; }


        public int? PriceFrom { get; set; }
        public int? PriceTo { get; set; }

        public int PriceUnit { get; set; } = 1000000000;

        public int? AreaFrom { get; set; }
        public int? AreaTo { get; set; }

        public int? NumOfBedroom { get; set; }
        public int? NumOfToilet { get; set; }
        public int? NumOfFloor { get; set; }
        public int? Width { get; set; }
        public int? StreetWidth { get; set; }
    }
}
