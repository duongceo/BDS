using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyRE.Core.Entities
{
    public class BaseQuery
    {
        public string Keyword { get; set; }
        public int Limit { get; set; } = 100;
        public int Page { get; set; } = 1;
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string KeywordLike
        {
            get
            {
                if (string.IsNullOrEmpty(this.Keyword)) return this.Keyword;
                var encodeForLike = this.Keyword.Replace("[", "[[]").Replace("%", "[%]");
                return "%" + encodeForLike + "%";
            }
        }
    }
}
