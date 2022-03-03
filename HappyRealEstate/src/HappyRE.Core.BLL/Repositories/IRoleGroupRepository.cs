using HappyRE.Core.Entities;
using HappyRE.Core.Entities.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HappyRE.Core.BLL.Repositories
{
    public interface IRoleGroupRepository: IBaseDPRepository<RoleGroup>
    {
        Task<int?> IU(RoleGroup obj);
        Task<Tuple<IEnumerable<RoleGroup>, int>> Search(BaseQuery query);
    }
}