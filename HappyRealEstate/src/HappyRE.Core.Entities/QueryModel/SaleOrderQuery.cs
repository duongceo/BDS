using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyRE.Core.Entities
{
    public class SaleOrderQuery: BaseQuery
    {
        public int? PropertyId { get; set; }
        public string SellBy { get; set; }
    }
}
