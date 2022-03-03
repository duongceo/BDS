using HappyRE.Core.Entities;
using HappyRE.Core.Entities.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HappyRE.Core.BLL.Repositories
{
    public interface IWardRepository: IBaseDPRepository<Ward>
    {
        Task<Tuple<IEnumerable<Ward>, int>> Search(CityQuery query);
        Task<int?> IU(Ward obj);
    }
}