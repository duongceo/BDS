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
    public class SysCode
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("Loại")]
        public string TableId { get; set; }

        [Required]
        [DisplayName("Tên")]
        public string Name { get; set; }
        [DisplayName("Mô tả")]
        public string Body { get; set; }
        public int BitMask { get; set; }
        public int Sort { get; set; }
    }

    public class SysCodeTable
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
