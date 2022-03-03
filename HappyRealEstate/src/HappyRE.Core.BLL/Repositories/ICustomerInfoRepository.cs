using HappyRE.Core.Entities.Model;
using HappyRE.Core.Entities.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HappyRE.Core.BLL.Repositories
{
    public interface ICustomerInfoRepository: IBaseDPRepository<CustomerInfo>
    {
        Task<int?> IU(CustomerInfo obj);
        Task<int?> UpdateByPhone(CustomerInfo obj);
        Task<List<CustomerInfo>> GetAlertBirtdayCustomers();
        Task<CustomerInfo> GetByPhone(string phone);
        Task<IEnumerable<CustomerInfoTransaction>> GetTransactions(string phone);
        Task<CustomerInfoSummary> GetSummary(string phone);
    }
}