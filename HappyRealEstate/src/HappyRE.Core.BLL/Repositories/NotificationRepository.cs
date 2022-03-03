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
using Dapper;
using System.Data;

namespace HappyRE.Core.BLL.Repositories
{
    public class NotificationRepository : BaseDPRepository<Notification>, INotificationRepository
    {
        public NotificationRepository(IUow uow)
            : base(uow)
        {
        }

        public async Task<Tuple<IEnumerable<Notification>, int>> Search(NotificationQuery query)
        {
            var p = new DynamicParameters();

            p.Add("total", dbType: DbType.Int32, direction: ParameterDirection.Output);
            p.Add("limit", query.Limit);
            p.Add("page", query.Page);
            p.Add("sendTo", query.SentTo);
            p.Add("fromDate", query.FromDate);
            p.Add("toDate", query.ToDate);
            p.Add("isRead", query.IsRead);
            p.Add("keyword", query.Keyword);
            var res = await this.Query<Notification>("msp_Notification_Search", p, System.Data.CommandType.StoredProcedure);
            var total = p.Get<int>("total");
            return new Tuple<IEnumerable<Notification>, int>(res, total);
        }

        public async Task<Tuple<IEnumerable<Notification>, int>> SearchAdmin(NotificationQuery query)
        {
            var p = new DynamicParameters();

            p.Add("total", dbType: DbType.Int32, direction: ParameterDirection.Output);
            p.Add("limit", query.Limit);
            p.Add("page", query.Page);
            p.Add("fromDate", query.FromDate);
            p.Add("toDate", query.ToDate);
            p.Add("keyword", query.Keyword);
            var res = await this.Query<Notification>("msp_Notification_AdminSearch", p, System.Data.CommandType.StoredProcedure);
            var total = p.Get<int>("total");
            return new Tuple<IEnumerable<Notification>, int>(res, total);
        }

        public async Task<int> UnReadCount(string sentTo)
        { 
            return await this.ExecuteScalar<int>("select count(*) from NotificationRead (nolock) where Deleted=0 and IsRead=0 and UserName = @sentTo",new{ sentTo }, System.Data.CommandType.Text);
        }

        public async Task<int?> IU(Notification obj)
        {
            var m = await this.GetById(obj.Id);
            if (m == null)
            {
                var r= await this.Insert(obj);
                if (r.HasValue)
                {
                    obj.Id = r.Value;
                    await uow.NotificationRead.AddList(obj);
                }
                return r;
            }
            else
            {
                m.Title = obj.Title;
                m.Details = obj.Details;
                await this.Update(m);
                return m.Id;
            }
        }

        public async Task<int> Delete(int id)
        {
            var q = @"delete NotificationRead where NotificationId =@id 
                    delete Notification where Id =@id";
            return await this.ExecuteScalar<int>(q, new { id }, CommandType.Text);
        }
    }
}
