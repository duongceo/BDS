using HappyRE.Core.Utils.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyRE.Core.Entities.ViewModel
{
    public class ImageFileViewModel
    {
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string SrcHtml { get; set; }
        public string SrcHtmlDisplay => string.IsNullOrEmpty(this.SrcHtml) ? "" : $"<ul class='img_list'>{this.SrcHtml}</ul>";
        public string DateChangedFriendly
        {
            get { return CreatedDate.ToFriendlyDate(); }
        }
        public string DateChangedDisplay
        {
            get
            {
                return CreatedDate.ToString("dd-MM-yyyy HH:mm:ss");
            }
        }
    }
}
