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
    public class RoleGroupRepository : BaseDPRepository<RoleGroup>, IRoleGroupRepository
    {
        public RoleGroupRepository(IUow uow)
            : base(uow)
        {
        }

        public async Task<Tuple<IEnumerable<RoleGroup>, int>> Search(BaseQuery query)
        {
            var p = new DynamicParameters();
            p.Add("total", dbType: DbType.Int32, direction: ParameterDirection.Output);
            p.Add("limit", query.Limit);
            p.Add("page", query.Page);
            p.Add("keyword", query.Keyword);
            var res = await this.Query<RoleGroup>("msp_RoleGroup_Search", p, System.Data.CommandType.StoredProcedure);
            var total = p.Get<int>("total");
            return new Tuple<IEnumerable<RoleGroup>, int>(res, total);

            //string conditions = "where Deleted=0 ";
            //if (string.IsNullOrEmpty(query.Keyword) == false) conditions += "and name = @keyword";
            //return await this.GetListPaged<RoleGroup>(query.Page, query.Limit, conditions, "id", new { keyword = query.Keyword });
        }

        public async Task<int?> IU(RoleGroup obj)
        {           
            var isExistsName = await this.IsExistsName<RoleGroup>("where id<>@id and name=@name", new { id = obj.Id, name = obj.Name });
            if (isExistsName == true) throw new BusinessException("Đã tồn tại!");
            var m = await this.GetById(obj.Id);
            if (m == null)
            {
                return await this.Insert(obj);
            }
            else
            {
                var isChanged = m.Roles != obj.Roles;
                m.Roles = obj.Roles;
                m.Name = obj.Name;
                await this.Update(m);

                if (isChanged) await ChangeUserRoles(m.Id, m.Roles);
                return m.Id;
            }
        }

        async Task ChangeUserRoles(int roleGroupId, int roles)
        {
            await this.Execute("msp_RoleGroup_UpdateUserRoles", new { roleGroupId,roles }, CommandType.StoredProcedure);
        }

        public override async Task DeleteCheck(RoleGroup obj)
        {
            var c = await this.ExecuteScalar<int>("select count(*) from UserProfile (nolock) where Deleted=0 and RoleGroupId=@roleGroupId", new { roleGroupId = obj.Id }, CommandType.Text);
            if (c > 0) throw new BusinessException($"Không thể xóa nhóm quyền này vì có {c} nhân viên trong nhóm này!");
        }
    }
}
