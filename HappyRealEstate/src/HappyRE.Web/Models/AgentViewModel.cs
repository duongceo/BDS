using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HappyRE.Core.MapModels;
using HappyRE.Core.MapModels.FrontEnd;

namespace HappyRE.Web.Models
{
    public class AgentViewModel
    {
        //Mock Test
        public AgentViewModel()
        {
            //Agent = new AgentDisplay
            //{
            //    ProfileId = 0,
            //    Name = "Bùi Phạm Thanh Tú",
            //    IsVerifiedIDCard = true,
            //    CertNo = "DA346341",
            //    Mobile = "0908686868",
            //    Address = "212",
            //    Street = "Lam Van Ben",
            //    Ward = "Tan Hung",
            //    District = "Quan 7",
            //    City = "Ho Chi Minh",
            //    Introduction = "Tự hào là một môi giới bất động sản uy tín về sản phẩm đất nền, Hoàng Thịnh luôn đi theo cam "

            //};
        }
        public AgentDisplay Agent { get; set; }
        public List<Core.MapModels.Search.PropertyItem> RentProperties;
        public List<Core.MapModels.Search.PropertyItem> SaleProperties;
        public Paging RentPaging { get; set; }
        public Paging SalePaging { get; set; }
        public AgentMessageViewModel LeftContactForm { get; set; }

        public string [] SalePropertiesFavorited { get; set; }
        public string [] RentPropertiesFavorited { get; set; }
    }
}