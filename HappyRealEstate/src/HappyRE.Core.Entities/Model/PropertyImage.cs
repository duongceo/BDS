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
    public class PropertyImage: BaseEntity
    {
        [Key]
        public int Id { get; set; }

        public int PropertyId { get; set; }
        public string Src { get; set; }
    }
}
