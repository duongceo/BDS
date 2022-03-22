using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HappyRE.Web.Models
{
    public class Breadcrumb
    {
        public bool Rent = false;
        public int CityId = 0;
        public int DistrictId = 0;
        public int PropertyTypeId = 0;
        public List<int> PropertyStyles = null;

        public int WardId = 0;
        public int StreetId = 0;
        public int ProjectId = 0;
		public int PlaceId = 0;

		public int FromPrice = 0;
        public int ToPrice = 0;
        public int FromArea = 0;
        public int ToArea = 0;

        public byte FromBedRoom = 0;
        public byte ToBedRoom = 0;

        public byte DirectionId = 0;
        public byte LegalId = 0;

        public int Days = 0;

        /// <summary>
        /// Hiển thị Lưu tìm kiếm
        /// </summary>
        public bool ShowAlertSearch = false;

        public List<Core.MapModels.LinkItem> Items = new List<Core.MapModels.LinkItem>();

        public bool HasPrice()
        {
            return (this.FromPrice > 0 || this.ToPrice > 0);
        }

        public bool HasArea()
        {
            return (this.FromArea > 0 || this.ToArea > 0);
        }

        public bool HasBedRoom()
        {
            return (this.FromBedRoom > 0 || this.ToBedRoom > 0);
        }

        public string GetPriceText()
        {
            string res = string.Empty;
            int f = this.FromPrice;
            int t = this.ToPrice;
            string funit = Core.Resources.Message.Msg_Price_Million;
            string tunit = funit;
            if (f >= 1000)
            {
                f = f / 1000;
                funit = Core.Resources.Message.Msg_Price_Billion;
            }
            if (t >= 1000)
            {
                t = t / 1000;
                tunit = Core.Resources.Message.Msg_Price_Billion;
            }

            if (f <= 0)
            {
                res = string.Format(Core.Resources.Message.Msg_Price_Less, t, tunit);
            }
            else if (t <= 0)
            {
                res = string.Format(Core.Resources.Message.Msg_Price_Greater, f, funit);
            }
            else
            {
                res = string.Format(Core.Resources.Message.Msg_Price_Range, f, funit, t, tunit);
            }

            return Core.Resources.Message.Msg_Price_Label + " " + res;
        }

        public string GetAreaText()
        {
            string res = string.Empty;
            int f = this.FromArea;
            int t = this.ToArea;
            if (f <= 0)
            {
                res = string.Format(Core.Resources.Message.Msg_Area_Less, t);
            }
            else if (t <= 0)
            {
                res = string.Format(Core.Resources.Message.Msg_Area_Greater, f);
            }
            else
            {

                res = string.Format(Core.Resources.Message.Msg_Area_Range, f, t);
            }
            return res;
        }

        public string GetBedRoomText()
        {
            string res = string.Empty;
            int f = this.FromBedRoom;
            int t = this.ToBedRoom;
			if (f == t)
			{
				res = string.Format(Core.Resources.Message.Breadcrumb_BedRoom, f);
			}
			else
			{
				res = string.Format(Core.Resources.Message.Breadcrumb_BedRoom, f.ToString() + "+");
				//res = string.Format(Core.Resources.Message.Msg_Area_Range, f.ToString() + "+");
			}
            return res;
        }

        public static Breadcrumb MapObject(Core.MapModels.SearchFilter filter)
        {
            return new Models.Breadcrumb()
            {
                Rent = filter.Rent,
                CityId = filter.CityId,
                DistrictId = filter.DistrictId,
                WardId = filter.WardId,
                StreetId = filter.StreetId,
                ProjectId = filter.ProjectId,
				PlaceId = filter.PlaceId,
				PropertyTypeId = filter.PropertyTypeId.GetValueOrDefault(0),
                PropertyStyles = filter.PropertyStyles,
                FromPrice = filter.FromPrice.GetValueOrDefault(0),
                ToPrice = filter.ToPrice.GetValueOrDefault(0),
                FromArea = filter.FromArea.GetValueOrDefault(0),
                ToArea = filter.ToArea.GetValueOrDefault(),
                FromBedRoom = filter.FromBedRoom.GetValueOrDefault(0),
                ToBedRoom = filter.ToBedRoom.GetValueOrDefault(0),
                DirectionId = filter.DirectionId.GetValueOrDefault(0),
                LegalId = filter.LegalId.GetValueOrDefault(0),
                Days = filter.Days.GetValueOrDefault(0)
            };
        }

        public static Breadcrumb MapObject(Core.MapModels.FrontEnd.PropertyDisplay obj)
        {
            return new Models.Breadcrumb()
            {
                Rent = obj.IsRent,
                CityId = obj.CityId,
                DistrictId = obj.DistrictId,
                WardId = obj.WardId,
                StreetId = obj.StreetId,
                ProjectId = obj.ProjectId.GetValueOrDefault(0),
				PlaceId = 0,
                PropertyTypeId = obj.PropertyTypeId,
                PropertyStyles = new List<int>() { obj.SubPropertyTypeId },
                FromPrice = 0,
                ToPrice = 0,
                FromArea = 0,
                ToArea = 0,
            };
        }
    }
}