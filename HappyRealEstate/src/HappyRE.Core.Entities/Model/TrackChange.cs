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
    public class TrackChange
    {
        [Key]
        public int Id { get; set; }
        public int TableKeyId { get; set; }
        public string TableId { get; set; }
        public string ColumnId { get; set; }
        [DisplayName("Thuộc tính")]
        public string ColumnName { get; set; }
        [DisplayName("Trước thay đổi")]
        public string Old { get; set; }
        [DisplayName("Sau thay đổi")]
        public string New { get; set; }
        [DisplayName("Người thay đổi")]
        public string UpdatedBy { get; set; }
        [DisplayName("Ngày thay đổi")]
        public DateTime? UpdatedDate { get; set; }

    }
}
