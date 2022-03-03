using HappyRE.Core.Entities.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HappyRE.Core.BLL.Repositories
{
    public interface IPropertyImageRepository: IBaseDPRepository<PropertyImage>
    {
        Task<int?> IU(PropertyImage obj);
        Task UpdateProductImages(int propertyId, List<string> images);
        Task<IEnumerable<string>> GetImages(int propertyId);
    }
}