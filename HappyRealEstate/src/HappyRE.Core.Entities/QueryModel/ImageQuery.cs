using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyRE.Core.Entities
{
    public class ImageFileQuery: BaseQuery
    {
        public string TableName { get; set; }
        public int TableKeyId { get; set; }
    }
}
