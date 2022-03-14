using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using System.ComponentModel;
using HappyRE.Core.Utils.Helpers;

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

    [Serializable]
    public class ImageFile : BaseEntity
    {
        [Key]
        public int Id { get; set; }
        public string TableName { get; set; }
        public int TableKeyId { get; set; }
        public string Src { get; set; }
        public int Size { get; set; } = 0;
        public bool IsMore { get; set; } = false;
        public string GroupCode { get; set; }

    }
}
