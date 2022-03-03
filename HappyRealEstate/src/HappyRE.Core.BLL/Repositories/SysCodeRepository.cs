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
    public class SysCodeRepository : BaseDPRepository<SysCode>, ISysCodeRepository
    {
        public SysCodeRepository(IUow uow)
            : base(uow)
        {
        }

        public async Task<Tuple<IEnumerable<SysCode>, int>> Search(SysCodeQuery query)
        {
            var p = new DynamicParameters();
            p.Add("total", dbType: DbType.Int32, direction: ParameterDirection.Output);
            p.Add("limit", query.Limit);
            p.Add("page", query.Page);
            p.Add("tableId", query.TableId);
            var res = await this.Query<SysCode>("msp_SysCode_Search", p, System.Data.CommandType.StoredProcedure);
            var total = p.Get<int>("total");
            return new Tuple<IEnumerable<SysCode>, int>(res, total);
        }

        public async Task<IEnumerable<KeyValueModel>> GetByBit(int bit, string tableId)
        {
            return await this.Query<KeyValueModel>("msp_SysCode_GetByBit", new { bit, tableId }, System.Data.CommandType.StoredProcedure);
        }

        public async Task<List<int>> GetBitMaskByBit(int bit, string tableId)
        {
            var res= await this.Query<SysCode>("msp_SysCode_GetByBit", new { bit, tableId }, System.Data.CommandType.StoredProcedure);
            return res.Select(x => x.BitMask).ToList();
        }

        public async Task<int?> IU(SysCode obj)
        {
            var m = await this.GetById(obj.Id);
            if (m == null)
            {
                var query = "select max(BitMask) from SysCode (nolock) where TableId=@tableId and Deleted=0";
                var lastBitMask= await this.ExecuteScalar<int>(query, new { tableId = obj.TableId }, System.Data.CommandType.Text);
                obj.BitMask = lastBitMask == 0? 1: lastBitMask * 2;
                return await this.Insert(obj);
            }
            else
            {
                //m.BitMask = obj.BitMask;
                m.Name = obj.Name;
                m.Body = obj.Body;
                //m.Sort = obj.Sort;
                await this.Update(m);
                return m.Id;
            }
        }
    }
}
