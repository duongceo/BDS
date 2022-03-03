using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyRE.Core.Entities
{
    public class HistoryLogQuery: BaseQuery
    {
        public string TableName { get; set; }
        public int? TableKeyId { get; set; }
        public string Action { get; set; }
        public string CreatedBy { get; set; }
    }

    public class TrackChangeQuery : BaseQuery
    {
        public string TableId { get; set; }
        public int? TableKeyId { get; set; }
        public string UpdatedBy { get; set; }
    }
}
