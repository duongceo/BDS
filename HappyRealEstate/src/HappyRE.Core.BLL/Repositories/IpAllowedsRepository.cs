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
using System.Data;
using Dapper;

namespace HappyRE.Core.BLL.Repositories
{
    public class IpAllowedsRepository : BaseDPRepository<IpAlloweds>, IIpAllowedsRepository
    {
        public IpAllowedsRepository(IUow uow)
            : base(uow)
        {
        }

        public async Task<Tuple<IEnumerable<IpAlloweds>, int>> Search(BaseQuery query)
        {
            var p = new DynamicParameters();
            p.Add("total", dbType: DbType.Int32, direction: ParameterDirection.Output);
            p.Add("limit", query.Limit);
            p.Add("page", query.Page);
            p.Add("keyword", query.Keyword);
            var res = await this.Query<IpAlloweds>("msp_IpAlloweds_Search", p, System.Data.CommandType.StoredProcedure);
            var total = p.Get<int>("total");
            return new Tuple<IEnumerable<IpAlloweds>, int>(res, total);

            //string conditions = "where Deleted=0 ";
            //if (string.IsNullOrEmpty(query.Keyword) == false) conditions += "and name = @keyword";
            //return await this.GetListPaged<IpAlloweds>(query.Page, query.Limit, conditions, "id", new { keyword = query.Keyword });
        }

        public bool IsValidIp(string ip)
        {
            var query = "Select count(*) from IpAlloweds (nolock) where Ip =@ip and Deleted=0";
            var t = this.ExecuteScalarNonAsync<int>(query, new { ip = ip }, System.Data.CommandType.Text);
            if (t > 0) return true;
            else
            {
                var t1 = this.ExecuteScalarNonAsync<int>("Select count(*) from IpAlloweds (nolock) where Deleted=0", new { }, System.Data.CommandType.Text);
                return t1 == 0;
            }
        }
        public async Task<int?> IU(IpAlloweds obj)
        {
            var m = await this.GetById(obj.Id);
            if (m == null)
            {
                return await this.Insert(obj);
            }
            else
            {
                m.Ip = obj.Ip;
                await this.Update(m);
                return m.Id;
            }
        }
    }
}
