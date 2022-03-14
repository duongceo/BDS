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
    public class Customer:BaseEntity
    {
        [Key]
        [NonTrack]
        public int Id { get; set; }

        [DisplayName("Họ tên")]
        [Required(ErrorMessage = "{0} không được để trống")]
        public string FullName { get; set; }
        [DisplayName("Số điện thoại")]
        [Required(ErrorMessage = "{0} không được để trống")]
        public string Phone { get; set; }
        [DisplayName("Số điện thoại khác")]
        public string ExtPhone { get; set; }
        [DisplayName("Giới tính")]
        public string Sex { get; set; }
        [DisplayName("Ngày sinh")]
        public DateTime? Birthday { get; set; }
        [DisplayName("Hình")]
        [NonTrack]
        public string Avatar { get; set; }
        [DisplayName("Phân loại")]
        [Required(ErrorMessage = "{0} không được để trống")]
        public int? ContractId { get; set; }
        [DisplayName("Trạng thái")]
        [Required(ErrorMessage = "{0} không được để trống")]
        public int StatusId { get; set; }
        [DisplayName("Nhu cầu")]
        [Required(ErrorMessage = "{0} không được để trống")]
        public int DemandId { get; set; }
        [DisplayName("Mục đích")]
        [Required(ErrorMessage = "{0} không được để trống")]
        public int TargetId { get; set; }
        [DisplayName("Nguồn khách")]
        [Required(ErrorMessage = "{0} không được để trống")]
        public int? SourceId { get; set; }
        [DisplayName("Hướng nhà")]
        public int? DirectionId { get; set; }
        [DisplayName("Tài chính từ")]
        [Required(ErrorMessage = "{0} không được để trống")]
        public decimal? BudgetFrom { get; set; }
        [DisplayName("đến")]
        [Required(ErrorMessage = "{0} không được để trống")]
        public decimal? BudgetTo { get; set; }
        [DisplayName("Diện tích tối thiểu")]
        public decimal? MinArea { get; set; }
        [DisplayName("Chiều rộng tối thiểu")]
        public decimal? MinWidth { get; set; }
        [DisplayName("Chiều dài tối thiểu")]
        public decimal? MinLength { get; set; }
        [DisplayName("Độ rộng của đường")]
        public float? StreetWidth { get; set; }
        [DisplayName("Đơn vị tiền")]
        public string CurrencyType { get; set; }
        [DisplayName("Phương thức")]
        public string CalcMethod { get; set; }
        [DisplayName("Số tầng")]
        public int? NumOfFloor { get; set; }
        [DisplayName("Số phòng")]
        public int? NumOfRoom { get; set; }
        [DisplayName("Ghi chú")]
        [Required(ErrorMessage = "{0} không được để trống")]
        public string Note { get; set; }
        [NonTrack]
        public bool IsForceHiddenPhone { get; set; }

        [NotMapped]
        public string RegionTargets { get; set; }
        [NotMapped]
        [NonTrack]
        public List<CustomerRegionTarget> RegionTarget { get; set; }

        [NotMapped]
        [NonTrack]
        public List<string> Images { get; set; } = new List<string>();
    }
}
