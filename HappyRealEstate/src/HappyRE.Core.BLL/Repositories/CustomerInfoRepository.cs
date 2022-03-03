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
    public class CustomerInfoRepository : BaseDPRepository<CustomerInfo>, ICustomerInfoRepository
    {
        public CustomerInfoRepository(IUow uow)
            : base(uow)
        {
        }
        public async Task<int?> IU(CustomerInfo obj)
        {
            var m = await this.GetById(obj.Id);
            if (m == null)
            {
                return await this.Insert(obj);
            }
            else
            {
                m.FullName = obj.FullName;
                m.Avatar = obj.Avatar;
                m.IdCardNumber = obj.IdCardNumber;
                m.Note = obj.Note;
                m.Birthday = obj.Birthday;
                m.AlertBirthDay = obj.AlertBirthDay;
                await this.Update(m);
                return m.Id;
            }
        }

        public async Task<int?> UpdateByPhone(CustomerInfo obj)
        {
            if (IsValidPhoneNumber(obj.Phone)==false) return 0;
            var u = await this.Query("select top 1 * from CustomerInfo (nolock) where Phone = @phone", new { obj.Phone }, CommandType.Text);
            if (u.Count() > 0)
            {
                var m = u.First();
                m.FullName = obj.FullName;
                m.Avatar = obj.Avatar;
                m.IdCardNumber = obj.IdCardNumber;
                m.Note = obj.Note;
                m.Birthday = obj.Birthday;
                m.AlertBirthDay = obj.AlertBirthDay;
                await this.Update(m);
                return m.Id;
            }
            else
            {
                return await this.Insert(obj);
            }
        }

        bool IsValidPhoneNumber(string phone)
        {
            return string.IsNullOrEmpty(phone) == false && phone.Length >= 9;
        }

        public async Task<CustomerInfo> GetByPhone(string phone)
        {
            var u=await this.Query<CustomerInfo>("select * from CustomerInfo (nolock) where Phone =@phone", new { phone }, CommandType.Text);
            return u.FirstOrDefault();
        }

         public async Task<List<CustomerInfo>> GetAlertBirtdayCustomers()
        {
            string q = @"with temp(FullName, Phone, [Address], Birthday, AlertBirthday)as(
                        select a.FullName, a.Phone, a.[Address], a.Birthday, a.AlertBirthday from CustomerInfo (nolock) a 
                        inner join SaleOrder (nolock) b on a.Phone=b.CustomerPhone 
                        where AlertBirthday=1 
                        union 
                        select a.FullName, a.Phone, a.[Address], a.Birthday, a.AlertBirthday from CustomerInfo (nolock) a 
                        inner join SaleOrder (nolock) b on a.Phone=b.OwnerPhone 
                        where AlertBirthday=1)
                        select * from temp 
                        where MONTH(Birthday) = MONTH(GETDATE()) and day(Birthday) = day(GETDATE())";
            var customers= await this.Query<CustomerInfo>(q, new { }, CommandType.Text);
            return customers.ToList();
        }

        public async Task<IEnumerable<CustomerInfoTransaction>> GetTransactions(string phone)
        {
            return await this.Query<CustomerInfoTransaction>("msp_CustomerInfo_Transaction", new { phone }, CommandType.StoredProcedure);
        }

        public async Task<CustomerInfoSummary> GetSummary(string phone)
        {
            var u= await this.Query<CustomerInfoSummary>("msp_CustomerInfo_Summary", new { phone }, CommandType.StoredProcedure);
            return u.FirstOrDefault();
        }
    }
}
