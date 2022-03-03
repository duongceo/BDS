using HappyRE.Core.Entities;
using HappyRE.Core.Entities.Model;
using HappyRE.Core.Entities.ViewModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HappyRE.Core.BLL.Repositories
{
    public interface ICustomerRepository: IBaseDPRepository<Customer>
    {
        Task<int?> IU(Customer obj);
        Task<Tuple<IEnumerable<CustomerListViewModel>, int>> Search(CustomerQuery query);
        Task<CustomerListViewModel> GetDetail(int id);
        Task<int> Merge_CustomerSearch(int id);
        Task<IEnumerable<CustomerListViewModel>> Export(CustomerQuery query);
        Task<int> MobileViewedToday();
        Task<int> ShowMobile(int customerId);
        Task<int> ForceHideMobile(int id, bool isForced);
        Task<bool> IsExistByPhone(string phone);
    }
}