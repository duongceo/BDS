using HappyRE.Core.Entities;
using HappyRE.Core.Entities.Model;
using HappyRE.Core.Entities.ViewModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HappyRE.Core.BLL.Repositories
{
    public interface ISysCodeRepository: IBaseDPRepository<SysCode>
    {
        Task<int?> IU(SysCode obj);
        Task<Tuple<IEnumerable<SysCode>, int>> Search(SysCodeQuery query);
        Task<IEnumerable<KeyValueModel>> GetByBit(int bit, string tableId);
        Task<List<int>> GetBitMaskByBit(int bit, string tableId);
    }
}