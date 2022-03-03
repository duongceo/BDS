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

namespace HappyRE.Core.BLL.Repositories
{
    public class CustomerRepository : BaseDPRepository<Customer>, ICustomerRepository
    {
        private readonly static int MaxViewMobileInDay = 10;
        public CustomerRepository(IUow uow)
            : base(uow)
        {
        }

        public async Task<Tuple<IEnumerable<CustomerListViewModel>, int>> Search(CustomerQuery query)
        {
            var p = new DynamicParameters();
            var userName = System.Threading.Thread.CurrentPrincipal.Identity.IsAuthenticated ? System.Threading.Thread.CurrentPrincipal.Identity.Name : "System";

            p.Add("total", dbType: DbType.Int32, direction: ParameterDirection.Output);
            p.Add("limit", query.Limit);
            p.Add("page", query.Page);
            p.Add("userName", userName);
            p.Add("keyword", query.Keyword);
            p.Add("contractId", query.ContractId_Filter);
            p.Add("statusId", query.StatusId_Filter);
            p.Add("demandId", query.DemandId_Filter);
            p.Add("sourceId", query.SourceId_Filter);
            p.Add("targetId", query.TargetId_Filter);
            p.Add("directionId", query.DirectionId_Filter);
            p.Add("cityId", query.CityId);
            p.Add("districtId", query.DistrictId);
            p.Add("price_bw", query.Price_bw);
            p.Add("area_bw", query.Area_bw);
            p.Add("width_bw", query.Width_bw);
            p.Add("streetWidth_bw", query.StreetWidth_bw);
            p.Add("numOfFloor_bw", query.NumOfFloor_bw);
            p.Add("numOfRoom_bw", query.NumOfRoom_bw);
            var res = await this.Query<CustomerListViewModel>("msp_Customer_Search", p, System.Data.CommandType.StoredProcedure);
            var total = p.Get<int>("total");
            return new Tuple<IEnumerable<CustomerListViewModel>, int>(res, total);
        }

        public async Task<CustomerListViewModel> GetDetail(int id)
        {
            var query = @"SELECT w.*,a.[Name] ContractName,d.[Name] SourceName,g.[Name] DirectionName,
                        case when w.CreatedBy =@userName then 1 else isNull(l.Id,0) end ViewedMobileToday
		                FROM  dbo.CustomerSearch (nolock) w 
		                left join dbo.SysCode (nolock) a on w.contractId = a.Id and a.TableId = 'ContractType' and a.Deleted=0 
		                left join dbo.SysCode (nolock) d on w.sourceId = d.Id and d.TableId = 'CustomerSourceType' and d.Deleted=0
		                left join dbo.SysCode (nolock) g on w.directionId = g.Id and g.TableId = 'PropertyDirectionType' and g.Deleted=0
                        left join dbo.CustomerShowMobileLog (nolock) l on w.Id = l.CustomerId and l.CreatedBy =@userName and CONVERT(nvarchar,l.CreatedDate, 102)  = CONVERT(nvarchar,getdate(), 102)
                        where w.Deleted=0 and w.Id =@id";
            var userName = System.Threading.Thread.CurrentPrincipal.Identity.IsAuthenticated ? System.Threading.Thread.CurrentPrincipal.Identity.Name : "System";
            var res = await this.Query<CustomerListViewModel>(query, new { id, userName }, CommandType.Text);
            return res.FirstOrDefault();
        }
        public async Task<int?> IU(Customer obj)
        {
            var m = await this.GetById(obj.Id);
            if (m == null)
            {
                var id= await this.Insert(obj);
                if(id.HasValue) await Merge_CustomerSearch(id.Value);
                return id;
            }
            else
            {
                await TrackChange(obj, m);

                m.FullName = obj.FullName;
                m.Phone = obj.Phone;
                m.ExtPhone = obj.ExtPhone;
                m.Sex = obj.Sex;
                m.Birthday = obj.Birthday;
                m.ContractId = obj.ContractId;
                m.StatusId = obj.StatusId;
                m.TargetId = obj.TargetId;
                m.SourceId = obj.SourceId;
                m.DirectionId = obj.DirectionId;
                m.DemandId = obj.DemandId;
                m.CurrencyType = obj.CurrencyType;
                m.CalcMethod = obj.CalcMethod;
                m.MinArea = obj.MinArea;
                m.MinLength = obj.MinLength;
                m.MinWidth = obj.MinWidth;
                m.StreetWidth = obj.StreetWidth;
                m.NumOfFloor = obj.NumOfFloor;
                m.NumOfRoom = obj.NumOfRoom;
                m.BudgetFrom = obj.BudgetFrom;
                m.BudgetTo = obj.BudgetTo;
                m.Note = obj.Note;
                m.Avatar = obj.Avatar;
                await this.Update(m);
                await Merge_CustomerSearch(m.Id);
                return m.Id;
            }
        }

        public async Task TrackChange(Customer _new, Customer _old)
        {
            try
            {
                List<TrackChange> tracks = new List<TrackChange>();
                foreach (var propertyInfo in _new.GetType().GetProperties().Where(p => !Attribute.IsDefined(p, typeof(NonTrack)) && Attribute.IsDefined(p, typeof(System.ComponentModel.DisplayNameAttribute))))
                {
                    // do stuff here
                    var n = propertyInfo.GetValue(_new);
                    var o = propertyInfo.GetValue(_old);
                    if (!Equals(n, o))
                    {
                        var t = new Entities.Model.TrackChange()
                        {
                            TableId = "Customer",
                            TableKeyId = _old.Id,
                            ColumnId = propertyInfo.Name,
                            ColumnName = propertyInfo.GetCustomAttributes(typeof(System.ComponentModel.DisplayNameAttribute), false).Cast<System.ComponentModel.DisplayNameAttribute>().Single().DisplayName,
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

        public async Task<int> Merge_CustomerSearch(int id)
        {
           return await this.ExecuteScalar<int>("msp_FetchCustomerSearch", new { id }, CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<CustomerListViewModel>> Export(CustomerQuery query)
        {
            query.Page = 1;
            query.Limit = 1000000;
            var res = await Search(query);
            return res.Item1;
        }

        #region ShowMobileLog
        public async Task<int> MobileViewedToday()
        {
            var userName = System.Threading.Thread.CurrentPrincipal.Identity.IsAuthenticated ? System.Threading.Thread.CurrentPrincipal.Identity.Name : "System";
            var res = await this.ExecuteScalar<int>("select count(distinct CustomerId) from CustomerShowMobileLog (nolock) where createdBy=@userName and ViewDate=@viewDate", new { userName,viewDate=DateTime.Today }, CommandType.Text);
            return res;
        }

        public async Task<int> ShowMobile(int customerId)
        {
            var viewed = await MobileViewedToday();
            if (viewed < MaxViewMobileInDay)
            {
                var userName = System.Threading.Thread.CurrentPrincipal.Identity.IsAuthenticated ? System.Threading.Thread.CurrentPrincipal.Identity.Name : "System";
                var sl = await this.ExecuteScalar<int>("select count(*) from CustomerShowMobileLog (nolock) where createdBy=@userName and CustomerId=@customerId and ViewDate=@viewDate", new { userName, customerId, viewDate = DateTime.Today }, CommandType.Text);
                if (sl == 0)
                {
                    return await this.ExecuteScalar<int>("insert into CustomerShowMobileLog(CustomerId,ViewDate,CreatedBy,CreatedDate,Deleted) values(@customerId,@viewDate,@createdBy,@createdDate,0)", new { customerId, viewDate = DateTime.Now, createdBy = userName, createdDate = DateTime.Now }, CommandType.Text);
                }
                return 0;
            }
            else
            {
                throw new BusinessException("Chỉ được xem 10 số điện thoại mỗi ngày");
            }
        }

        public async Task<int> ForceHideMobile(int id,bool isForced)
        {
            var customer = await this.GetById(id);
            if (customer != null)
            {
                customer.IsForceHiddenPhone = isForced;
                var res= await this.Update(customer);
                await Merge_CustomerSearch(id);
                return res;
            }
            return 0;
        }

        public async Task<bool> IsExistByPhone(string phone)
        {
            var c = await this.ExecuteScalar<int>("select count(*) from Customer (nolock) where Phone = @phone and Deleted=0", new { phone }, CommandType.Text);
            return c > 0;
        }
        #endregion
    }
}
