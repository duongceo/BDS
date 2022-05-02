using log4net;
using MBN.Utils;
using HappyRE.Core.BLL.Services;
using HappyRE.Core.BLL.Utils;
using HappyRE.Core.Entities;
using HappyRE.Core.Entities.Model;
using HappyRE.Core.Entities.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Data;

namespace HappyRE.Core.BLL.Repositories
{
    public class UserProfileRepository : BaseDPRepository<UserProfile>, IUserProfileRepository
    {
        private static readonly ILog _log = LogManager.GetLogger("UserProfileRepository");
        private readonly static int LimitUser = int.Parse(WebUtils.AppSettings("LIMIT_USER", "0"));

        public UserProfileRepository(IUow uow)
            : base(uow)
        {
        }

        public IEnumerable<KeyValueUserModel> GetAll()
        {
            return this.QueryNonAsync<KeyValueUserModel>("select UserName Id, FullName, isnull(a.DepartmentId,0) ParentId from UserProfile (nolock) a left join Department (nolock) b on a.DepartmentId = b.Id where a.Deleted=0 and a.UserStatus=0 and b.Deleted=0", new { }, CommandType.Text);
        }
        public async Task<UserProfile> GetByUserName(string userName)
        {
            var l= await this.Query<UserProfile>("select top 1 * from UserProfile (nolock) where UserName =@userName and Deleted=0", new { userName }, CommandType.Text);
            return l.FirstOrDefault();
        }

        public async Task<IEnumerable<UserDepartmentModel>> GetUserOrDepartment(string keyword)
        {
            if (string.IsNullOrEmpty(keyword))
            {
                return new List<UserDepartmentModel>();
            }
            var encodeForLike = keyword.Replace("[", "[[]").Replace("%", "[%]");
            keyword= "%" + encodeForLike + "%";
            return await this.Query<UserDepartmentModel>(@"select UserName Id, (FullName + ' ('+ UserName+')') Name, 1 Type 
                    from UserProfile (nolock)
                    where Deleted=0 and UserStatus=0 and (UserName like @keyword or FullName like @keyword)
                    union all
                    select cast(Id as varchar(10)) Id, Name, 2 Type
                    from Department (nolock)
                    where Deleted=0 and Name like @keyword", new { keyword }, CommandType.Text);
        }

        public async Task<IEnumerable<KeyValueDisplayModel>> GetListKeyValue(string keyword, string id)
        {
            if (string.IsNullOrEmpty(keyword) && string.IsNullOrEmpty(id))
            {
                return new List<KeyValueDisplayModel>();
            }else if (string.IsNullOrEmpty(id)==false && string.IsNullOrEmpty(keyword) == true)
            {
                return await this.Query<KeyValueDisplayModel>(@"select UserName Id, (FullName + ' - ' + Mobile + ' ('+ UserName+')') Name
                    from UserProfile (nolock)
                    where UserName= @id", new { id }, CommandType.Text);
            }
            var encodeForLike = keyword.Replace("[", "[[]").Replace("%", "[%]");
            keyword = "%" + encodeForLike + "%";
            return await this.Query<KeyValueDisplayModel>(@"select UserName Id, (FullName + ' - ' + Mobile + ' ('+ UserName+')') Name
                    from UserProfile (nolock)
                    where Deleted=0 and UserStatus=0 and (UserName like @keyword or FullName like @keyword or Mobile like @keyword)", new { keyword }, CommandType.Text);
        }

        public async Task<IEnumerable<string>> GetUserByDeparments(List<string> ids)
        {
            List<int> d = new List<int>();
            List<string> u = new List<string>();
            foreach (var id in ids)
            {
                var isNumeric = int.TryParse(id, out _);
                if (isNumeric) d.Add(int.Parse(id));
                else u.Add(id);
            }
            string query = "select UserName from UserProfile (nolock) where Deleted=0 and UserStatus=0 and UserName in @u or DepartmentId in (select Id from Department (nolock) where Deleted=0 and Id in @d)";
            return await this.Query<string>(query, new { u, d }, CommandType.Text);
        }

        public async Task<bool> IsNotCheckIP(string userName)
        {
            var query = @"select count(*) from AspNetUsers (nolock) a
                        inner join AspNetUserRoles (nolock) b on a.Id= b.UserId                      
                        where a.UserName=@userName and b.RoleId in ('IP_ACCESS')";
            var l = await this.ExecuteScalar<int>(query, new { userName }, CommandType.Text);
            return l > 0;
        }

        public async Task<bool> IsLockedOut(string userName)
        {
            var query = @"select count(*) from UserProfile (nolock) where UserName =@userName and (Deleted=1 or UserStatus=1)";
            var l = await this.ExecuteScalar<int>(query, new { userName }, CommandType.Text);
            return l > 0;
        }

        public async Task<IEnumerable<string>> GetUsersInRole(string roleName)
        {
            var q = @"select Distinct a.UserName
                    from AspNetUsers a
                    inner join AspNetUserRoles b on a.Id = b.UserId
                    inner join AspNetRoles c on c.Id = b.RoleId
                    where c.Name=@roleName";
            return await this.Query<string>(q, new { roleName }, CommandType.Text);
        }
        public async Task<Tuple<IEnumerable<UserProfileListViewModel>, int>> Search(UserProfileQuery query)
        {
            var p = new DynamicParameters();
            
            p.Add("total", dbType: DbType.Int32, direction: ParameterDirection.Output);
            p.Add("limit", query.Limit);
            p.Add("page", query.Page);
            p.Add("keyword", query.Keyword);
            p.Add("departmentId", query.DepartmentId_Filter);
            p.Add("levelId", query.LevelId_Filter);
            p.Add("roleGroupId", query.RoleGroupId_Filter);
            p.Add("userstatus", query.UserStatus_Filter);
            var res= await this.Query<UserProfileListViewModel>("msp_UserProfile_Search", p, System.Data.CommandType.StoredProcedure);
            var total = p.Get<int>("total");
            return new Tuple<IEnumerable<UserProfileListViewModel>, int>(res, total);
        }

        public async Task<IEnumerable<UserProfileListViewModel>> Export(UserProfileQuery query)
        {
            query.Page = 1;
            query.Limit = 1000000;
            var res = await Search(query);
            return res.Item1;
        }
        public async Task<int?> IU(UserProfile obj)
        {
            var m = await this.GetById(obj.Id);
            if (m == null)
            {
                if (LimitUser > 0)
                {
                    var canAdd = await base.CheckInsertLimit<UserProfile>(LimitUser);
                    if (canAdd == false)
                    {
                        throw new BusinessException($"Gói tài khoản sử dụng đang giới hạn quản lý {LimitUser} nhân viên!");
                    }
                }

                obj.ActiveDate = DateTime.Now;
                var res= await this.Insert(obj);
                await ChangeRoleGroup(obj.UserId, obj.RoleGroupId.Value);
                return res;
            }
            else
            {
                bool isChangeRole = m.RoleGroupId != obj.RoleGroupId;
                bool isChangestatus = m.UserStatus != obj.UserStatus;
                m.FullName = obj.FullName;
                m.Email = obj.Email;
                m.Mobile = obj.Mobile;
                m.DepartmentId = obj.DepartmentId;
                m.LevelId = obj.LevelId;
                m.RoleGroupId = obj.RoleGroupId;
                m.UserStatus = obj.UserStatus;
                m.Avatar = obj.Avatar;
                await this.Update(m);
                if(isChangeRole) await ChangeRoleGroup(m.UserId, obj.RoleGroupId.Value);
                if(isChangestatus) await ChangeUserStatus(m.UserId, m.UserStatus);
                return m.Id;
            }
        }

        async Task ChangeUserStatus(string userId, int userStatus)
        {
            var query = @"update AspNetUsers
                            set LockoutEnabled=@LockoutEnabled
                            where Id=@id";
            await this.ExecuteScalar<int>(query, new { id = userId, LockoutEnabled = userStatus==1?true:false }, CommandType.Text);
        }

        public async Task ChangeUserDepartment(int id, int departmentId)
        {
            var query = @"update UserProfile
                            set DepartmentId=@departmentId
                            where Id=@id";
            await this.ExecuteScalar<int>(query, new { id = id, departmentId = departmentId }, CommandType.Text);
        }
        async Task ChangeRoleGroup(string userId, int roleGroupId)
        {
            await this.Execute("msp_User_UpdateRoles", new { userId, roleGroupId }, CommandType.StoredProcedure);
        }

        public async Task TranferAccount(int fromUser, int toUser)
        {
            await this.Execute("msp_User_Tranfer", new { fromUserId= fromUser, toUserId= toUser }, CommandType.StoredProcedure);
        }
        public async Task<int> Delete(int id)
        {
            var user = await this.GetById(id);
            if (user != null)
            {
                var res = await this.Delete(new UserProfile() { Id = id, UserName= user.UserName }, false);
                if (res >= 0)
                {
                    var query = @"update AspNetUsers
                            set LockoutEnabled=1
                            where Id=@id";
                    return await this.ExecuteScalar<int>(query, new { id = user.UserId }, CommandType.Text);
                }
                return res;
            }
            return 0;
        }

        public override async Task DeleteCheck(UserProfile obj)
        {
            var c = await this.ExecuteScalar<int>("select count(*) from PropertySearch (nolock) where Deleted=0 and PostedBy=@userName", new { userName = obj.UserName }, CommandType.Text);
            if (c > 0) throw new BusinessException($"Hãy chuyển dữ liệu trước khi xóa nhân viên này vì có dữ liệu liên quan!");
            c = await this.ExecuteScalar<int>("select count(*) from CustomerSearch (nolock) where Deleted=0 and CreatedBy=@userName", new { userName = obj.UserName }, CommandType.Text);
            if (c > 0) throw new BusinessException($"Hãy chuyển dữ liệu trước khi xóa nhân viên này vì có dữ liệu liên quan!");
            c = await this.ExecuteScalar<int>("select count(*) from SaleOrder (nolock) where Deleted=0 and (PostedBy=@userName or SellBy=@userName)", new { userName = obj.UserName }, CommandType.Text);
            if (c > 0) throw new BusinessException($"Hãy chuyển dữ liệu trước khi xóa nhân viên này vì có dữ liệu liên quan!");

        }
    }
}
