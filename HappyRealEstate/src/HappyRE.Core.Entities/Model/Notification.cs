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
    public class Notification: BaseEntity
    {
        [Key]
        public int Id { get; set; }
        [DisplayName("Gửi đến")]
        public string SentTo { get; set; }
        [DisplayName("Tiêu đề")]
        public string Title { get; set; }
        [DisplayName("Chi tiết")]
        public string Details { get; set; }
        [DisplayName("Link chi tiết")]
        public string DetailsURL { get; set; }
        [DisplayName("Loại")]
        public string NotificationType { get; set; }
        [DisplayName("Mã")]
        public string Code { get; set; }

        [NotMapped]
        public bool IsAll => this.SentTo == null;

        [NotMapped]
        [DisplayName("Đã xem")]
        public bool IsRead { get; set; }

        [DisplayName("Ngày gửi")]
        [NotMapped]
        public string TimeDisplay => this.CreatedDate.ToFriendlyDate();

        [NotMapped]
        public List<string> SendTos { get; set; } = new List<string>();
        [NotMapped]
        public string SentToDisplay => this.SentTo == null ? "" : this.SentTo.Length > 200 ? this.SentTo.Substring(0, 200) + ".." : this.SentTo;

    }
}
