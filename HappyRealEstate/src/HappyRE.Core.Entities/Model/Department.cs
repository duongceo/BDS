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
    public class Department: BaseEntity
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("Mã phòng ban")]
        public string Code { get; set; }
        [DisplayName("Tên phòng ban")]
        public string Name { get; set; }
        [DisplayName("Trưởng phòng")]
        public int? ManagerId { get; set; }
        [DisplayName("Ngày thành lập")]
        public DateTime? StartDate { get; set; }
        [DisplayName("Thông tin")]
        public string Note { get; set; }
        [DisplayName("Số điện thoại")]
        public string Phone { get; set; }
    }
}
