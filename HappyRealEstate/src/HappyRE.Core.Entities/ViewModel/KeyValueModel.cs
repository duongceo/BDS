using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyRE.Core.Entities.ViewModel
{
    public class KeyValueModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ParentId { get; set; }
    }

    public class KeyValueUserModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int ParentId { get; set; }
    }

    public class KeyValueDisplayModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
