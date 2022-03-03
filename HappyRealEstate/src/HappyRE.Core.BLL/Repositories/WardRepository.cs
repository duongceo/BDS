﻿using log4net;
using HappyRE.Core.Entities.Model;
using HappyRE.Core.Entities.ViewModel;
using HappyRE.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HappyRE.Core.Entities;
using Dapper;
using System.Data;

namespace HappyRE.Core.BLL.Repositories
{
    public class WardRepository : BaseDPRepository<Ward>, IWardRepository
    {
        public WardRepository(IUow uow)
            : base(uow)
        {
        }

        public async Task<Tuple<IEnumerable<Ward>, int>> Search(CityQuery query)
        {
            var p = new DynamicParameters();
            p.Add("total", dbType: DbType.Int32, direction: ParameterDirection.Output);
            p.Add("limit", query.Limit);
            p.Add("page", query.Page);
            p.Add("keyword", query.Keyword);
            p.Add("cityId", query.CityId);
            p.Add("districtId", query.DistrictId);
            var res = await this.Query<Ward>("msp_Ward_Search", p, System.Data.CommandType.StoredProcedure);
            var total = p.Get<int>("total");
            return new Tuple<IEnumerable<Ward>, int>(res, total);

            //string conditions = "where Deleted=0 ";
            //if (string.IsNullOrEmpty(query.Keyword) == false) conditions += " and name like @keyword";
            //if (query.CityId.HasValue && query.CityId.Value > 0) conditions += " and cityId = @cityId";
            //if (query.DistrictId.HasValue && query.DistrictId.Value > 0) conditions += " and DistrictId = @districtId";
            //return await this.GetListPaged<Ward>(query.Page, query.Limit, conditions, "id", new { keyword = query.KeywordLike, districtId = query.DistrictId, cityId = query.CityId });
        }
        public async Task<int?> IU(Ward obj)
        {
            var m = await this.GetById(obj.Id);
            if (m == null)
            {
                return await this.Insert(obj);
            }
            else
            {
                m.CityId = obj.CityId;
                m.DistrictId = obj.DistrictId;
                m.Prefix = obj.Prefix;
                m.Name = obj.Name;
                await this.Update(m);
                return m.Id;
            }
        }
    }
}