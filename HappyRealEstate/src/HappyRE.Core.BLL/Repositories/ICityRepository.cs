using HappyRE.Core.Entities;
using HappyRE.Core.Entities.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HappyRE.Core.BLL.Repositories
{
    public interface ICityRepository: IBaseDPRepository<City>
    {
        IEnumerable<City> GetAll();
        Task<int?> IU(City obj);
        Task<Tuple<IEnumerable<City>, int>> Search(CityQuery query);
    }
}