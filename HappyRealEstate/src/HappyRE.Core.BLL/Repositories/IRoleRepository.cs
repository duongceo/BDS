using HappyRE.Core.Entities;
using HappyRE.Core.Entities.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HappyRE.Core.BLL.Repositories
{
    public interface IRoleRepository: IBaseDPRepository<Role>
    {
        Task<IEnumerable<Role>> GetRolesByParent(int id);
        Task<int?> IU(Role obj);
        Task<Tuple<IEnumerable<Role>, int>> Search(BaseQuery query);
    }
}