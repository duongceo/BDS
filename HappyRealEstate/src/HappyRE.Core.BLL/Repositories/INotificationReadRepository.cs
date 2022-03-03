using HappyRE.Core.Entities.Model;
using System.Threading.Tasks;

namespace HappyRE.Core.BLL.Repositories
{
    public interface INotificationReadRepository: IBaseDPRepository<NotificationRead>
    {
        Task<int?> IU(NotificationRead obj);
        Task AddList(Notification data);
        Task<int?> Read(NotificationRead obj);
    }
}