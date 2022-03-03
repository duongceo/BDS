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
    public class StreetListViewModel: BaseEntity
    {
        [Key]
        public int Id { get; set; }      
        public int CityId { get; set; }
       
        public int DistrictId { get; set; }

        [DisplayName("Tên đường")]
        public string Name { get; set; }
        public string Prefix { get; set; }
        [DisplayName("Quận huyện")]
        public string DistrictName { get; set; }
        [DisplayName("Thành phố")]
        public string CityName { get; set; }
    }
}
