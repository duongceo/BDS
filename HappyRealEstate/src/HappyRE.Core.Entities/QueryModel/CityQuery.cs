﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyRE.Core.Entities
{
    public class CityQuery: BaseQuery
    {
        public int? CityId { get; set; }
        public int? DistrictId { get; set; }
    }

    public class LocationQuery
    {
        public int CityId { get; set; }
        public int? DistrictId { get; set; }
        public int? WardId { get; set; }
        public int? StreetId { get; set; }
    }
}
