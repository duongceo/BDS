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
    public class IpAlloweds: BaseEntity
    {
        [Key]
        public int Id { get; set; }
        public string Ip { get; set; }
    }
}
