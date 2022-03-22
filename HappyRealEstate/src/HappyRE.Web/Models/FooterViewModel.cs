using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HappyRE.Core.Entities;

namespace HappyRE.Web.Models
{
    public class FooterViewModel
    {
        public List<CMS_Category> Footer { get; set; }
        public List<CMS_Category> Popular { get; set; }
    }
}