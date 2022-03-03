using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using System.ComponentModel;
using HappyRE.Core.Entities.ViewModel;

namespace HappyRE.Core.Entities.Model
{
    [Serializable]
    public class DepartmentListViewModel: BaseEntity
    {
        [Key]
        [ExportIgnore]
        public int Id { get; set; }
        [DisplayName("#")]
        public int RowNumber { get; set; }

        [DisplayName("Mã phòng ban")]
        public string Code { get; set; }
        [DisplayName("Tên phòng ban")]
        public string Name { get; set; }
        [DisplayName("Quản lý phòng")]
        public string ManagerName { get; set; }
        [DisplayName("Số điện thoại")]
        public string Phone { get; set; }
        [ExportIgnore]
        public int? ManagerId { get; set; }
        [DisplayName("Ngày thành lập")]
        [ExportIgnore]
        public DateTime? StartDate { get; set; }
        [DisplayName("Ngày thành lập")]
        public string StartDateDisplay => this.StartDate?.ToString("dd/MM/yyyy");

        [DisplayName("Thông tin")]
        public string Note { get; set; }
        
    }
}
