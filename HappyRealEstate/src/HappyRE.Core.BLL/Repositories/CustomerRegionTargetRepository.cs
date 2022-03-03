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
    public class CustomerRegionTargetRepository : BaseDPRepository<CustomerRegionTarget>, ICustomerRegionTargetRepository
    {
        public CustomerRegionTargetRepository(IUow uow)
            : base(uow)
        {
        }
        public async Task<Tuple<IEnumerable<CustomerRegionTargetViewModel>, int>> LocationSearch(CityQuery query)
        {
            var p = new DynamicParameters();
            p.Add("total", dbType: DbType.Int32, direction: ParameterDirection.Output);
            p.Add("limit", query.Limit);
            p.Add("page", query.Page);
            p.Add("keyword", query.Keyword);
            p.Add("cityId", query.CityId);
            p.Add("districtId", query.DistrictId);
            var res = await this.Query<CustomerRegionTargetViewModel>("msp_Location_Search", p, System.Data.CommandType.StoredProcedure);
            var total = p.Get<int>("total");
            return new Tuple<IEnumerable<CustomerRegionTargetViewModel>, int>(res, total);
        }

        public async Task<IEnumerable<CustomerRegionTargetViewModel>> GetByCustomer(int customerId)
        {
            var query = @"select w.*,a.Name CityName, b.Name WardName, c.Name StreetName, d.Name DistrictName 
                        from CustomerRegionTarget (nolock) w 
                         left join City (nolock) a on w.CityId = a.Id 
                        left join District (nolock) d on w.DistrictId = d.Id 
                        left join Ward (nolock) b on w.WardId = b.Id 
                        left join Street (nolock) c on w.StreetId = c.Id 
                        where w.Deleted=0 and w.CustomerId =@customerId";
            return await this.Query<CustomerRegionTargetViewModel>(query, new { customerId }, System.Data.CommandType.Text);
        }
        public async Task<int?> IU(CustomerRegionTarget obj)
        {
            var m = await this.GetById(obj.Id);
            if (m == null)
            {
                return await this.Insert(obj);
            }
            else
            {
                m.CustomerId = obj.CustomerId;
                m.CityId = obj.CityId;
                m.DistrictId = obj.DistrictId;
                m.WardId = obj.WardId;
                m.StreetId = obj.StreetId;
                await this.Update(m);
                return m.Id;
            }
        }
    }
}
