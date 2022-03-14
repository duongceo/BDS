using HappyRE.Core.Entities.ViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyRE.Core.Entities.Model
{
    public class BaseEntity
    {
        [DisplayName("Người tạo")]
        [NonTrack]
        [ExportIgnore]
        public string CreatedBy { get; set; }
        [DisplayName("Người cập nhật")]
        [NonTrack]
        [ExportIgnore]
        public string UpdatedBy { get; set; }

        [DisplayName("Ngày tạo")]
        [ExportIgnore]
        [NonTrack]
        public DateTime CreatedDate { get; set; }

        [DisplayName("Ngày cập nhật")]
        [ExportIgnore]
        [NonTrack]
        public DateTime? UpdatedDate { get; set; }

        [DisplayName("Ngày tạo")]
        [NotMapped]
        [NonTrack]
        public string Created_Date => this.CreatedDate.ToString("dd/MM/yyyy HH:mm:ss");

        [DisplayName("Ngày cập nhật")]
        [NotMapped]
        [NonTrack]
        [ExportIgnore]
        public string Modified_Date => this.UpdatedDate.HasValue? this.UpdatedDate.Value.ToString("dd/MM/yyyy HH:mm:ss"):"";
    }
}
