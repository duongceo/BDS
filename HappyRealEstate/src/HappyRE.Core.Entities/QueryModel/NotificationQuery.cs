using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyRE.Core.Entities
{
    public class NotificationQuery: BaseQuery
    {
        public string SentTo { get; set; }
        public bool? IsRead { get; set; }
    }
}
