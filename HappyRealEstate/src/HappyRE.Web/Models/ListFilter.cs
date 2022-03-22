using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HappyRE.Web.Models
{
    [Serializable]
    public class ListFilter
    {
        public bool Rent { get; set; }
        public string CityUrl { get; set; }
        public string DistrictUrl { get; set; }
        public int CityId { get; set; }
        public int DistrictId { get; set; }
        public int StreetId { get; set; }
        public int WardId { get; set; }
        public int ProjectId { get; set; }
        public int PlaceId { get; set; }
        public int PropertyTypeId { get; set; }
        

        public string PropUrl { get; set; }
        public int[] psid { get; set; }
        public int? fp { get; set; }
        public int? tp { get; set; }
        public int? fa { get; set; }
        public int? ta { get; set; }
        public byte? fbr { get; set; }
        public byte? tbr { get; set; }
        public byte? dt { get; set; }
        public byte? lg { get; set; }
        public byte? d { get; set; }
        public string s { get; set; }

        public int? cp { get; set; }

        public int rid { get; set; }
        public int rtid { get; set; }
        public List<string> polyenc { get; set; }
        public string q { get; set; }
    }
}