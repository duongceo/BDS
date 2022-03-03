using HappyRE.Core.Entities;
using HappyRE.Core.Entities.Model;
using HappyRE.Core.Entities.ViewModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HappyRE.Core.BLL.Repositories
{
    public interface IDepartmentRepository: IBaseDPRepository<Department>
    {
        IEnumerable<KeyValueModel> GetAll();
        Task<int?> IU(Department obj);
        Task<int?> UpdateManager(Department obj);
        Task<Tuple<IEnumerable<DepartmentListViewModel>, int>> Search(BaseQuery query);
        Task<IEnumerable<DepartmentListViewModel>> Export(BaseQuery query);
    }
}