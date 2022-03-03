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
            p.Add("sellBy", query.SellBy);
            p.Add("keyword", query.Keyword);
            p.Add("fromDate", query.FromDate);
            p.Add("toDate", query.ToDate);

            var res = await this.Query<SaleOrderListViewModel>("msp_SaleOrder_Search", p, System.Data.CommandType.StoredProcedure);
            var total = p.Get<int>("total");
            return new Tuple<IEnumerable<SaleOrderListViewModel>, int>(res, total);
        }

        public async Task<SaleOrderListViewModel> GetDetail(int id)
        {
            var query = @"SELECT w.*,a.PropertyNumber
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
                if (id.HasValue) await Merge_SaleOrderSearch(id.Value);
                if (id.HasValue) await IUCustomer(obj);
                return id;
            }
            else
            {
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
                //m.CustomerPhone = obj.CustomerPhone;
                m.AlertCustomerBirthDay = obj.AlertCustomerBirthDay;
                m.AlertOwnerBirthDay = obj.AlertOwnerBirthDay;
                //m.OrderDate = obj.OrderDate;

                await this.Update(m);
                await Merge_SaleOrderSearch(m.Id);

                await IUCustomer(obj);
                return m.Id;
            }

            
        }

        public async Task<int?> UpdateCustomer(SaleOrder obj)
        {
            var m = await this.GetById(obj.Id);
            if (m != null)
            {
                m.CustomerPhone = obj.CustomerPhone;
                m.OrderDate = obj.OrderDate;

                await this.Update(m);

                await Merge_SaleOrderSearch(m.Id);
                await IUCustomer(m);
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
    }
}
