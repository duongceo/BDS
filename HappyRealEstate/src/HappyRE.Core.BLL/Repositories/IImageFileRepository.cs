using HappyRE.Core.Entities;
using HappyRE.Core.Entities.Model;
using HappyRE.Core.Entities.ViewModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HappyRE.Core.BLL.Repositories
{
    public interface IImageFileRepository : IBaseDPRepository<ImageFile>
    {
        Task<Tuple<IEnumerable<ImageFileViewModel>, int>> Search(ImageFileQuery query);
        Task<IEnumerable<string>> GetImages(ImageFileQuery query);
        Task<int?> IU(ImageFile obj);
        Task UpdateImages(ImageFileQuery query, List<string> images);
        Task AddImages(ImageFileQuery query, List<string> images);
    }
}