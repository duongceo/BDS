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
using Dapper;
using System.Net.Http;

namespace HappyRE.Core.BLL.Repositories
{
    public class SaleOrderRepository : BaseDPRepository<SaleOrder>, ISaleOrderRepository
    {
        public SaleOrderRepository(IUow uow)
            : base(uow)
        {
        }

        public async Task<Tuple<IEnumerable<SaleOrderListViewModel>, int>> Search(SaleOrderQuery query)
        {
            var p = new DynamicParameters();
            p.Add("total", dbType: DbType.Int32, direction: ParameterDirection.Output);
            p.Add("limit", query.Limit);
            p.Add("page", query.Page);
            p.Add("propertyId", query.PropertyId);
            p.Add("postedBy", query.PostedBy);
            p.Add("sellBy", query.SellBy);
            p.Add("keyword", query.Keyword);
            p.Add("fromDate", query.FromDate);
            p.Add("toDate", query.ToDate);
            p.Add("userName", query.UserName);

            var res = await this.Query<SaleOrderListViewModel>("msp_SaleOrder_Search", p, System.Data.CommandType.StoredProcedure);
            var total = p.Get<int>("total");
            return new Tuple<IEnumerable<SaleOrderListViewModel>, int>(res, total);
        }

        public async Task<SaleOrderListViewModel> GetDetail(int id)
        {
            var query = @"SELECT w.*,a.Code PropertyNumber
		                FROM  dbo.SaleOrderSearch (nolock) w 
		                inner join dbo.PropertySearch (nolock) a on w.propertyId = a.Id and a.Deleted=0
                        where w.Deleted=0 and w.Id =@id";
            var userName = System.Threading.Thread.CurrentPrincipal.Identity.IsAuthenticated ? System.Threading.Thread.CurrentPrincipal.Identity.Name : "System";
            var res = await this.Query<SaleOrderListViewModel>(query, new { id, userName }, CommandType.Text);
            return res.FirstOrDefault();
        }
        public async Task<int?> IU(SaleOrder obj)
        {
            var m = await this.GetById(obj.Id);
            if (m == null)
            {
                var id = await this.Insert(obj);
                if(id.HasValue && id > 0)
                {
                    await uow.ImageFile.UpdateImages(new ImageFileQuery()
                    {
                        TableName = "SaleOrder_Owner",
                        TableKeyId = id.Value,
                    }, obj.OwnerImages);

                    await uow.ImageFile.UpdateImages(new ImageFileQuery()
                    {
                        TableName = "SaleOrder_Customer",
                        TableKeyId = id.Value,
                    }, obj.CustomerImages);
                }
                if (id.HasValue) await Merge_SaleOrderSearch(id.Value);
                if (id.HasValue) await IUCustomer(obj);
                if (string.IsNullOrEmpty(obj.CustomerPhone)==false && obj.OrderDate!=null) Hangfire.BackgroundJob.Enqueue<ISaleOrderRepository>(x => x.NotifyToAdmin(obj));
                if (id.HasValue && string.IsNullOrEmpty(obj.SellBy)==false)
                {
                    obj.Id = id.Value;
                    Hangfire.BackgroundJob.Enqueue<ISaleOrderRepository>(x => x.NotifyToSeller(obj));
                }
                return id;
            }
            else
            {
                await TrackChange(obj, m);
                bool changeSellBy = obj.SellBy != m.SellBy;
                m.OwnerAddress = obj.OwnerAddress;
                m.OwnerAvatar = obj.OwnerAvatar;
                m.OwnerBirthday = obj.OwnerBirthday;
                m.OwnerIDNumber = obj.OwnerIDNumber;
                m.OwnerName = obj.OwnerName;
                m.OwnerPhone = obj.OwnerPhone;
                m.OwnerTargetId = obj.OwnerTargetId;
                m.PropertyId = obj.PropertyId;
                m.RewardPoint = obj.RewardPoint;
                m.SellBy = obj.SellBy;
                m.PostedBy = obj.PostedBy;
                m.TotalAmount = obj.TotalAmount;
                m.RewardPoint = obj.RewardPoint;
                m.CustomerName = obj.CustomerName;
                m.CustomerAddress = obj.CustomerAddress;
                m.CustomerAvatar = obj.CustomerAvatar;
                m.CustomerBirthday = obj.CustomerBirthday;
                m.CustomerIDNumber = obj.CustomerIDNumber;
                m.CustomerTargetId = obj.CustomerTargetId;
                m.CustomerPhone = obj.CustomerPhone;
                m.AlertCustomerBirthDay = obj.AlertCustomerBirthDay;
                m.AlertOwnerBirthDay = obj.AlertOwnerBirthDay;
                m.OrderDate = obj.OrderDate;

                await this.Update(m);

                await uow.ImageFile.UpdateImages(new ImageFileQuery()
                {
                    TableName = "SaleOrder_Owner",
                    TableKeyId = m.Id
                }, obj.OwnerImages);

                await uow.ImageFile.UpdateImages(new ImageFileQuery()
                {
                    TableName = "SaleOrder_Customer",
                    TableKeyId = m.Id
                }, obj.CustomerImages);

                await Merge_SaleOrderSearch(m.Id);

                await IUCustomer(obj);

                if(changeSellBy) Hangfire.BackgroundJob.Enqueue<ISaleOrderRepository>(x => x.NotifyToSeller(obj));
                return m.Id;
            }

            
        }

        public async Task NotifyToSeller(SaleOrder obj)
        {
            var q = "select count(*) from Notification (nolock) where Deleted=0 and Type=6 and Code = @code";
            var sl = await this.ExecuteScalar<int>(q, new { code = obj.Id.ToString() }, CommandType.Text);
            if (sl == 0 && obj.Id > 0 && string.IsNullOrEmpty(obj.SellBy)==false)
            {
                var client = new HttpClient();
                await client.GetAsync($"https://tqt.batdongsanhanhphuc.vn/job/AlertNewSO?soId={obj.Id}&sellBy={obj.SellBy}");
            }
        }

        public async Task NotifyToAdmin(SaleOrder obj)
        {
            //var q = "select count(*) from Notification (nolock) where Deleted=0 and Type=7 and Code = @code";
            //var sl = await this.ExecuteScalar<int>(q, new { code = obj.Id.ToString() }, CommandType.Text);
            //if (sl == 0 && obj.Id > 0)
            //{

            //}
            if (obj != null && obj.Id > 0)
            {
                var client = new HttpClient();
                await client.GetAsync($"https://tqt.batdongsanhanhphuc.vn/job/AlertNewSOToAdmin?soId={obj.Id}");
            }
        }

        public async Task<int?> UpdateCustomer(SaleOrder obj)
        {
            var m = await this.GetById(obj.Id);

            if (m.OwnerPhone == obj.CustomerPhone && string.IsNullOrEmpty(obj.CustomerPhone)==false) throw new BusinessException("SĐT chủ nhà và khách hàng trùng nhau, vui lòng kiểm tra lại!");
            var k = await this.GetById(obj.Id);
            if (m != null)
            {
                bool isChanged = m.CustomerPhone != obj.CustomerPhone;
                m.CustomerPhone = obj.CustomerPhone;
                m.OrderDate = obj.OrderDate;
                await TrackChange(m,k);
                await this.Update(m);

                await Merge_SaleOrderSearch(m.Id);
                await IUCustomer(m);

                if(isChanged) Hangfire.BackgroundJob.Enqueue<ISaleOrderRepository>(x => x.NotifyToAdmin(m));
                return m.Id;
            }
            return null;
        }

        public async Task<int> Merge_SaleOrderSearch(int id)
        {
            return await this.ExecuteScalar<int>("msp_FetchSaleOrderSearch", new { id }, CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<SaleOrderListViewModel>> Export(SaleOrderQuery query)
        {
            query.Page = 1;
            query.Limit = 1000000;
            var res = await Search(query);
            return res.Item1;
        }

        async Task IUCustomer(SaleOrder obj)
        {
            await uow.CustomerInfo.UpdateByPhone(new CustomerInfo()
            {
                Phone = obj.OwnerPhone,
                FullName = obj.OwnerName,
                Birthday = obj.OwnerBirthday,
                IdCardNumber = obj.OwnerIDNumber,
                Address = obj.OwnerAddress,
                Avatar = obj.OwnerAvatar,
                AlertBirthDay = obj.AlertOwnerBirthDay

            });

            await uow.CustomerInfo.UpdateByPhone(new CustomerInfo()
            {
                Phone = obj.CustomerPhone,
                FullName = obj.CustomerName,
                Birthday = obj.CustomerBirthday,
                IdCardNumber = obj.CustomerIDNumber,
                Address = obj.CustomerAddress,
                Avatar = obj.CustomerAvatar,
                AlertBirthDay = obj.AlertCustomerBirthDay
            });
        }

        public async Task TrackChange(SaleOrder _new, SaleOrder _old)
        {
            try
            {
                List<TrackChange> tracks = new List<TrackChange>();
                foreach (var soInfo in _new.GetType().GetProperties().Where(p => !Attribute.IsDefined(p, typeof(NonTrack)) && Attribute.IsDefined(p, typeof(System.ComponentModel.DisplayNameAttribute))))
                {
                    // do stuff here
                    var n = soInfo.GetValue(_new);
                    var o = soInfo.GetValue(_old);
                    if (!Equals(n, o))
                    {
                        var t = new Entities.Model.TrackChange()
                        {
                            TableId = "SaleOrder",
                            TableKeyId = _old.Id,
                            ColumnId = soInfo.Name,
                            ColumnName = soInfo.GetCustomAttributes(typeof(System.ComponentModel.DisplayNameAttribute), false).Cast<System.ComponentModel.DisplayNameAttribute>().Single().DisplayName,
                            Old = o?.ToString(),
                            New = n?.ToString(),
                            UpdatedBy = System.Threading.Thread.CurrentPrincipal.Identity.IsAuthenticated ? System.Threading.Thread.CurrentPrincipal.Identity.Name : "System",
                            UpdatedDate = DateTime.Now
                        };

                        await this.ExecuteScalar<int>(@"INSERT INTO [HappyRETracking].[dbo].[TrackChange]
                                                   ([TableId]
                                                    ,[TableKeyId]
                                                   ,[ColumnId]
                                                   ,[ColumnName]
                                                   ,[New]
                                                   ,[Old]
                                                   ,[UpdatedBy]
                                                   ,[UpdatedDate])
                                             VALUES
                                                   (@TableId
                                                   ,@TableKeyId
                                                   ,@ColumnId
                                                   ,@ColumnName
                                                   ,@New
                                                   ,@Old
                                                   ,@UpdatedBy
                                                   ,@UpdatedDate)", new { t.TableId, t.TableKeyId, t.ColumnId, t.ColumnName, t.New, t.Old, t.UpdatedBy, t.UpdatedDate }, CommandType.Text);
                    }
                }
            }
            catch (Exception ex) { }
        }

        public override async Task DeleteAfter(SaleOrder obj)
        {
            if (obj.Id > 0) await Merge_SaleOrderSearch(obj.Id);
        }
    }
}
