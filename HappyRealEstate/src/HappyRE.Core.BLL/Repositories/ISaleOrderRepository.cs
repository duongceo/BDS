using HappyRE.Core.Entities;
using HappyRE.Core.Entities.Model;
using HappyRE.Core.Entities.ViewModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HappyRE.Core.BLL.Repositories
{
    public interface ISaleOrderRepository: IBaseDPRepository<SaleOrder>
    {
        Task<IEnumerable<SaleOrderListViewModel>> Export(SaleOrderQuery query);
        Task<SaleOrderListViewModel> GetDetail(int id);
        Task<int?> IU(SaleOrder obj);
        Task<int?> UpdateCustomer(SaleOrder obj);
        Task<int> Merge_SaleOrderSearch(int id);
        Task<Tuple<IEnumerable<SaleOrderListViewModel>, int>> Search(SaleOrderQuery query);
    }
}