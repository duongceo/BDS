using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyRE.Core.Entities
{
    public class CustomerQuery: BaseQuery
    {
        public int? ContractId_Filter { get; set; }
        [DisplayName("Trạng thái")]        
        public int? StatusId_Filter { get; set; }
        [DisplayName("Nhu cầu")]       
        public int? DemandId_Filter { get; set; }
        [DisplayName("Mục đích")]       
        public int? TargetId_Filter { get; set; }
        [DisplayName("Nguồn khách")]      
        public int? SourceId_Filter { get; set; }
        [DisplayName("Hướng nhà")]
        public int? DirectionId_Filter { get; set; }

        [DisplayName("Diện tích")]
        public string Area_bw { get; set; }
        [DisplayName("Chiều rộng")]
        public string Width_bw { get; set; }
        [DisplayName("Chiều dài")]
        public string Length_bw { get; set; }
        [DisplayName("Đường rộng")]
        public string StreetWidth_bw { get; set; }
        [DisplayName("Đơn vị tiền")]
        public string CurrencyType_Filter { get; set; }
        [DisplayName("Phương thức")]
        public string CalcMethod_Filter { get; set; }
        [DisplayName("Số tầng")]
        public string NumOfFloor_bw { get; set; }
        [DisplayName("Số phòng")]
        public string NumOfRoom_bw { get; set; }
        [DisplayName("Tỉnh thành")]
        public int? CityId { get; set; }
        [DisplayName("Quận huyện")]
        public int? DistrictId { get; set; }

        [DisplayName("Tài chính")]
        public string Price_bw { get; set; }
    }
}
