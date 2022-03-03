using log4net;
using HappyRE.Core.Entities.Model;
using HappyRE.Core.Entities.ViewModel;
using HappyRE.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HappyRE.Core.Entities;
using System.Data;

namespace HappyRE.Core.BLL.Repositories
{
    public class NotificationReadRepository : BaseDPRepository<NotificationRead>, INotificationReadRepository
    {
        public NotificationReadRepository(IUow uow)
            : base(uow)
        {
        }
        public async Task<int?> IU(NotificationRead obj)
        {
            var m = await this.GetById(obj.Id);
            if (m == null)
            {
                return await this.Insert(obj);
            }
            else
            {
                await this.Update(m);
                return m.Id;
            }
        }

        public async Task<int?> Read(NotificationRead obj)
        {
            var query = "Update NotificationRead set IsRead=1, ReadDate =@readDate where NotificationId =@NotificationId and UserName = @UserName";
            return await this.ExecuteScalar<int>(query, new { readDate =DateTime.Now, NotificationId= obj.NotificationId, UserName = obj.UserName }, System.Data.CommandType.Text);
        }

        public async Task AddList(Notification data)
        {
            string createdBy = System.Threading.Thread.CurrentPrincipal.Identity.IsAuthenticated ? System.Threading.Thread.CurrentPrincipal.Identity.Name : "System";
            var query = "Insert into NotificationRead(NotificationId,UserName,CreatedBy,CreatedDate) select @NotificationId, UserName, @CreatedBy,@CreatedDate from UserProfile (nolock) where Deleted=0 and UserStatus=0";
            if (data.SendTos!=null && data.SendTos.Count > 0)
            {
                query = "Insert into NotificationRead(NotificationId,UserName,CreatedBy,CreatedDate) select @NotificationId,UserName, @CreatedBy,@CreatedDate from UserProfile (nolock) where UserName in @list";
            }
            await this.ExecuteScalar<int>(query, new { @NotificationId=data.Id, list = data.SendTos, CreatedBy = createdBy, CreatedDate = DateTime.Now }, System.Data.CommandType.Text);
        }
    }
}
