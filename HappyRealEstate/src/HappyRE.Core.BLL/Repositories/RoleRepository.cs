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
    public class RoleRepository : BaseDPRepository<Role>, IRoleRepository
    {
        public RoleRepository(IUow uow)
            : base(uow)
        {
        }

        public async Task<Tuple<IEnumerable<Role>, int>> Search(BaseQuery query)
        {
            var p = new DynamicParameters();
            p.Add("total", dbType: DbType.Int32, direction: ParameterDirection.Output);
            p.Add("limit", query.Limit);
            p.Add("page", query.Page);
            p.Add("keyword", query.Keyword);
            var res = await this.Query<Role>("msp_Role_Search", p, System.Data.CommandType.StoredProcedure);
            var total = p.Get<int>("total");
            return new Tuple<IEnumerable<Role>, int>(res, total);

            //string conditions = "where Deleted=0 ";
            //if (string.IsNullOrEmpty(query.Keyword) == false) conditions += "and name = @keyword";
            //return await this.GetListPaged<Role>(query.Page, query.Limit, conditions, "id", new { keyword = query.Keyword });
        }

        public async Task<IEnumerable<Role>> GetRolesByParent(int id)
        {
            return await this.Query<Role>("select * from dbo.Role (nolock) where Id & @id <> 0", new { id }, System.Data.CommandType.Text);
        }

        public async Task<int?> IU(Role obj)
        {
            var m = await this.GetById(obj.Id);
            if (m == null)
            {
                return await this.Insert(obj);
            }
            else
            {
                m.RoleId = obj.RoleId;
                m.Name = obj.Name;
                await this.Update(m);
                return m.Id;
            }
        }
    }
}
