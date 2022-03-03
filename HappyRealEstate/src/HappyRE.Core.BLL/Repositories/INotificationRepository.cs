using HappyRE.Core.Entities;
using HappyRE.Core.Entities.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HappyRE.Core.BLL.Repositories
{
    public interface INotificationRepository: IBaseDPRepository<Notification>
    {
        Task<int?> IU(Notification obj);
        Task<Tuple<IEnumerable<Notification>, int>> Search(NotificationQuery query);
        Task<Tuple<IEnumerable<Notification>, int>> SearchAdmin(NotificationQuery query);
        Task<int> UnReadCount(string sentTo);
        Task<int> Delete(int id);
    }
}