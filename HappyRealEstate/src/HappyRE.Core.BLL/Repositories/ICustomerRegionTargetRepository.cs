using HappyRE.Core.Entities;
using HappyRE.Core.Entities.Model;
using HappyRE.Core.Entities.ViewModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HappyRE.Core.BLL.Repositories
{
    public interface ICustomerRegionTargetRepository: IBaseDPRepository<CustomerRegionTarget>
    {
        Task<int?> IU(CustomerRegionTarget obj);      
        Task<Tuple<IEnumerable<CustomerRegionTargetViewModel>, int>> LocationSearch(CityQuery query);
        Task<IEnumerable<CustomerRegionTargetViewModel>> GetByCustomer(int customerId);
    }
}