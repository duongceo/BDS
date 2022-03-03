using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using System.ComponentModel;

namespace HappyRE.Core.Entities.ViewModel
{
    [Serializable]
    public class CustomerRegionTargetViewModel
    {
        [Key]
        public int Id { get; set; }      
        public int CustomerId { get; set; }
       
        public int CityId { get; set; }
        public int DistrictId { get; set; }
        public int? WardId { get; set; }
        public int? StreetId { get; set; }

        [DisplayName("Đường")]
        public string StreetName { get; set; }
        [DisplayName("Quận huyện")]
        public string DistrictName { get; set; }
        [DisplayName("Thành phố")]
        public string CityName { get; set; }
        [DisplayName("Phường xã")]
        public string WardName { get; set; }
    }
}
