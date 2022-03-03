using HappyRE.Core.Entities;
using HappyRE.Core.Entities.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HappyRE.Core.BLL.Repositories
{
    public interface IStreetRepository: IBaseDPRepository<Street>
    {
        Task<Tuple<IEnumerable<StreetListViewModel>, int>> Search(CityQuery query);
        Task<int?> IU(Street obj);
    }
}