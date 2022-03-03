using log4net;
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
    public class StreetRepository : BaseDPRepository<Street>, IStreetRepository
    {
        public StreetRepository(IUow uow)
            : base(uow)
        {
        }

        public async Task<Tuple<IEnumerable<StreetListViewModel>, int>> Search(CityQuery query)
        {
            var p = new DynamicParameters();
            p.Add("total", dbType: DbType.Int32, direction: ParameterDirection.Output);
            p.Add("limit", query.Limit);
            p.Add("page", query.Page);
            p.Add("keyword", query.Keyword);
            p.Add("cityId", query.CityId);
            p.Add("districtId", query.DistrictId);
            var res = await this.Query<StreetListViewModel>("msp_Street_Search", p, System.Data.CommandType.StoredProcedure);
            var total = p.Get<int>("total");
            return new Tuple<IEnumerable<StreetListViewModel>, int>(res, total);
        }

        public async Task<int?> IU(Street obj)
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
