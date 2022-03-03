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
    public class Role : BaseEntity
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("Tên quyền")]
        public string RoleId { get; set; }
        [DisplayName("Mô tả")]
        public string Name { get; set; }
    }
}
