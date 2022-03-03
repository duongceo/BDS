using HappyRE.Core.Entities;
using HappyRE.Core.Entities.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HappyRE.Core.BLL.Repositories
{
    public interface IDistrictRepository: IBaseDPRepository<District>
    {
        IEnumerable<District> GetAll();
        Task<Tuple<IEnumerable<District>, int>> Search(CityQuery query);
        Task<int?> IU(District obj);
    }
}