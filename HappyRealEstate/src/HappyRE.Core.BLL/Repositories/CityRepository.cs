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
    public class CityRepository : BaseDPRepository<City>, ICityRepository
    {
        public CityRepository(IUow uow)
            : base(uow)
        {
        }

        public IEnumerable<City> GetAll()
        {
           return this.QueryNonAsync<City>("select Id,Name from City (nolock)",new { },CommandType.Text);
        }

        public async Task<Tuple<IEnumerable<City>, int>> Search(CityQuery query)
        {
            var p = new DynamicParameters();
            p.Add("total", dbType: DbType.Int32, direction: ParameterDirection.Output);
            p.Add("limit", query.Limit);
            p.Add("page", query.Page);
            p.Add("keyword", query.Keyword);
            var res = await this.Query<City>("msp_City_Search", p, System.Data.CommandType.StoredProcedure);
            var total = p.Get<int>("total");
            return new Tuple<IEnumerable<City>, int>(res, total);
        }
        public async Task<int?> IU(City obj)
        {
            var m = await this.GetById(obj.Id);
            if (m == null)
            {
                return await this.Insert(obj);
            }
            else
            {
                m.Code = obj.Code;
                m.Name = obj.Name;
                await this.Update(m);
                return m.Id;
            }
        }
    }
}
