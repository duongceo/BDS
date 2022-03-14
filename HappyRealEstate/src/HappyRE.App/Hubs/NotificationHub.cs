using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using HappyRE.Core.BLL.Repositories;
using log4net;
using Microsoft.AspNet.SignalR;

namespace HappyRE.App
{
    public class NotificationHub : Hub
    {
        private static readonly ILog _log = LogManager.GetLogger("NotificationHub");
        private static readonly ConcurrentDictionary<string, UserHubModels> Users =
        new ConcurrentDictionary<string, UserHubModels>(StringComparer.InvariantCultureIgnoreCase);

        private IUow _uow = null;
        private IUow uow
        {
            get { return _uow ?? (_uow = Core.Entities.ObjectFactory.GetInstance<Uow>()); }
        }

        //private NotifEntities context = new NotifEntities();

        //Logged Use Call  
        public async Task GetNotification()
        {
            try
            {
                string loggedUser = Context.User.Identity.Name;

                //Get TotalNotification  
                string totalNotif = await LoadNotifData(loggedUser);

                //Send To  
                UserHubModels receiver;
                if (Users.TryGetValue(loggedUser, out receiver))
                {
                    var cid = receiver.ConnectionIds.FirstOrDefault();
                    var context = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
                    context.Clients.Client(cid).broadcaastNotif(totalNotif);
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }

        public void SendNotificationToList(Core.Entities.Model.Notification data, bool? isAll = false)
        {
            if (isAll == false)
            {
                foreach (var item in data.SendTos) {
                    SendNotification(data,item);
                }
            }
            else
            {
                var context = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
                context.Clients.All.subcribleNotify(new NotificationResult()
                {
                    Id= data.Id,
                    Title= data.Title,
                    CreatedDate= DateTime.Now
                });
            }
        } 
            //Specific User Call  
         public void SendNotification(Core.Entities.Model.Notification data, string SentTo)
         {
            try
            {
                if (string.IsNullOrEmpty(SentTo) == true) return;
                //Get TotalNotification  
                //string totalNotif = await LoadNotifData(SentTo);

                //Send To  
                _log.Info(SentTo);
                UserHubModels receiver;
                if (Users.TryGetValue(SentTo, out receiver))
                {
                    var cid = receiver.ConnectionIds.FirstOrDefault();
                    _log.Info("Cid:" + cid);
                    var context = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
                    context.Clients.Client(cid).subcribleNotify(new NotificationResult()
                    {
                        Id = data.Id,
                        Title = data.Title,
                        CreatedDate = DateTime.Now
                    });
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }

        private async Task<string> LoadNotifData(string userId)
        {
            int total = 0;
            total= await uow.Notification.UnReadCount(userId);
            return total.ToString();
        }

        public override Task OnConnected()
        {
            string userName = Context.User.Identity.Name;
            string connectionId = Context.ConnectionId;

            var user = Users.GetOrAdd(userName, _ => new UserHubModels
            {
                UserName = userName,
                ConnectionIds = new HashSet<string>()
            });

            lock (user.ConnectionIds)
            {
                user.ConnectionIds.Add(connectionId);
                if (user.ConnectionIds.Count == 1)
                {
                    Clients.Others.userConnected(userName);
                }
            }

            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            string userName = Context.User.Identity.Name;
            string connectionId = Context.ConnectionId;

            UserHubModels user;
            Users.TryGetValue(userName, out user);

            if (user != null)
            {
                lock (user.ConnectionIds)
                {
                    user.ConnectionIds.RemoveWhere(cid => cid.Equals(connectionId));
                    if (!user.ConnectionIds.Any())
                    {
                        UserHubModels removedUser;
                        Users.TryRemove(userName, out removedUser);
                        Clients.Others.userDisconnected(userName);
                    }
                }
            }

            return base.OnDisconnected(stopCalled);
        }
    }
}