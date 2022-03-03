using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using HappyRE.Core.BLL.Repositories;
using HappyRE.Core.Entities;
using HappyRE.Core.BLL.Utils;

namespace HappyRE.Api.Hubs
{
    public class NotificationHub : Hub
    {
        private IUow _uow = null;
        private IUow uow
        {
            get { return _uow ?? (_uow = Core.Entities.ObjectFactory.GetInstance<Uow>()); }
        }
        public void Hello()
        {
            Clients.All.hello();
        }
    }
}