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
    public class DistrictRepository : BaseDPRepository<District>, IDistrictRepository
    {
        public DistrictRepository(IUow uow)
            : base(uow)
        {
        }

        public IEnumerable<District> GetAll()
        {
            return this.QueryNonAsync<District>("select Id,Name, CityId from District (nolock)", new { }, CommandType.Text);
        }

        public async Task<Tuple<IEnumerable<District>, int>> Search(CityQuery query)
        {
            var p = new DynamicParameters();
            p.Add("total", dbType: DbType.Int32, direction: ParameterDirection.Output);
            p.Add("limit", query.Limit);
            p.Add("page", query.Page);
            p.Add("keyword", query.Keyword);
            p.Add("cityId", query.CityId);
            var res = await this.Query<District>("msp_District_Search", p, System.Data.CommandType.StoredProcedure);
            var total = p.Get<int>("total");
            return new Tuple<IEnumerable<District>, int>(res, total);

            //string conditions = "where Deleted=0 ";
            //if (string.IsNullOrEmpty(query.Keyword) == false) conditions += " and name like @keyword";
            //if(query.CityId.HasValue && query.CityId.Value>0) conditions += " and cityId = @cityId";
            //return await this.GetListPaged<District>(query.Page, query.Limit, conditions, "id", new { keyword = query.KeywordLike, cityId=query.CityId });
        }
        public async Task<int?> IU(District obj)
        {
            var m = await this.GetById(obj.Id);
            if (m == null)
            {
                return await this.Insert(obj);
            }
            else
            {
                m.CityId = obj.CityId;
                m.Prefix = obj.Prefix;
                m.Name = obj.Name;
                await this.Update(m);
                return m.Id;
            }
        }

        public override async Task DeleteCheck(District obj)
        {
            var c = await this.ExecuteScalar<int>("select count(*) from Property (nolock) where Deleted=0 and DistrictId=@districtId", new { districtId = obj.Id }, CommandType.Text);
            if (c > 0) throw new BusinessException($"Không thể xóa quận/huyện này vì có {c} BĐS!");
        }
    }
}
