using HappyRE.Core.Entities;
using HappyRE.Core.Entities.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HappyRE.Core.BLL.Repositories
{
    public interface IIpAllowedsRepository: IBaseDPRepository<IpAlloweds>
    {
        Task<int?> IU(IpAlloweds obj);
        bool IsValidIp(string ip);
        Task<Tuple<IEnumerable<IpAlloweds>, int>> Search(BaseQuery query);
    }
}