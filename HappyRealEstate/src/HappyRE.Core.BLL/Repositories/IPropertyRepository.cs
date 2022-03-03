using HappyRE.Core.Entities;
using HappyRE.Core.Entities.Model;
using HappyRE.Core.Entities.ViewModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HappyRE.Core.BLL.Repositories
{
    public interface IPropertyRepository:IBaseDPRepository<Property>
    {
        IEnumerable<KeyValueModel> GetAll();
        Task<IEnumerable<PropertyListViewModel>> Export(PropertyQuery query);
        Task<int?> IU(Property obj);
        Task<int> Merge_PropertySearch(int id);
        Task<Tuple<IEnumerable<PropertyListViewModel>, int>> Search(PropertyQuery query);
        Task<IEnumerable<KeyValueDisplayModel>> GetListKeyValue(string keyword,string id);
        Task<PropertyListViewModel> GetDetail(int id);
        Task<int> MobileViewedToday();
        Task<int> ShowMobile(int propertyId);
        Task<int> ForceHideMobile(int id, bool isForced);
        Task<bool> IsExistsCode(Property obj);
        Task<bool> IsExistsAddress(Property obj);
    }
}