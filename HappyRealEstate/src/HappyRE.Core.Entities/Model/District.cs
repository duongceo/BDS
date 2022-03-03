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
    public class District: BaseEntity
    {
        [Key]
        public int Id { get; set; }
        [DisplayName("Tỉnh thành")]
        public int CityId { get; set; }

        [DisplayName("Tên Quận/Huyện")]
        [Required]
        public string Name { get; set; }
        public string Prefix { get; set; }
    }
}
