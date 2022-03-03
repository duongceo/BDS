using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using System.ComponentModel;

namespace HappyRE.Core.Entities.Model
{
    [Serializable]
    public class CustomerRegionTarget : BaseEntity
    {
        [Key]
        public int Id { get; set; }
        [DisplayName("Khách hàng")]
        public int CustomerId { get; set; }

        [DisplayName("Thành phố")]
        [Required]
        public int CityId { get; set; }
        [DisplayName("Quận huyện")]
        public int? DistrictId { get; set; }
        [DisplayName("Phường xã")]
        public int? WardId { get; set; }

        [DisplayName("Đường")]
        public int? StreetId { get; set; }
    }
}
