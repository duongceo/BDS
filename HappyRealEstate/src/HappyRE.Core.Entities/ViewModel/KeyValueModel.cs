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
        public string FullName { get; set; }
        public string Name => $"{FullName} ({Id})";
        public int ParentId { get; set; }
    }

    public class KeyValuePropertyModel
    {
        public int Id { get; set; }
        public string PostedBy { get; set; }
        public string Name { get; set; }//(this.RawName ?? "").Replace("</br>", ", ").Replace("<b class=\"highlight\">", "").Replace("</b>", "").Replace("Số nhà: ", "").Replace(", Đường: ", " ").Replace("Quận: ", "").Replace("Tỉnh/TP: ", "").Replace(":","");
        public int ParentId { get; set; }
    }

    public class KeyValueDisplayModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class Select2Result
    {
        public int id { get; set; }
        public string text { get; set; }
        public string postedby { get; set; }
    }

    public class Select2Pagination
    {
        public bool more { get; set; }
    }

    public class Select2Results
    {
        public List<Select2Result> results { get; set; }
        public Select2Pagination pagination { get; set; }
    }
}
