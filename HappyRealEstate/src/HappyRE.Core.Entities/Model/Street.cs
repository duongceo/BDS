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
    public class Street: BaseEntity
    {
        [Key]
        public int Id { get; set; }
        [DisplayName("Thành phố")]
        [Required]
        public int CityId { get; set; } = 1;
        [DisplayName("Quận huyện")]
        [Required]
        public int DistrictId { get; set; }

        [DisplayName("Tên đường")]
        [Required]
        public string Name { get; set; }
        public string Prefix { get; set; }
    }
}
