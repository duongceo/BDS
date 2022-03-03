using HappyRE.Core.Entities;
using HappyRE.Core.Entities.Model;
using HappyRE.Core.Entities.ViewModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HappyRE.Core.BLL.Repositories
{
    public interface IUserProfileRepository: IBaseDPRepository<UserProfile>
    {
        IEnumerable<KeyValueUserModel> GetAll();
        Task<UserProfile> GetByUserName(string userName);
        Task<bool> IsAdmin(string userName);
        Task<int?> IU(UserProfile obj);
        Task<Tuple<IEnumerable<UserProfileListViewModel>, int>> Search(UserProfileQuery query);
        Task<IEnumerable<KeyValueDisplayModel>> GetListKeyValue(string keyword, string id);
        Task<IEnumerable<UserDepartmentModel>> GetUserOrDepartment(string keyword);
        Task<IEnumerable<string>> GetUserByDeparments(List<string> ids);
        Task<IEnumerable<string>> GetUsersInRole(string roleName);
        Task TranferAccount(int fromUser, int toUser);
        Task<IEnumerable<UserProfileListViewModel>> Export(UserProfileQuery query);
        Task<int> Delete(int id);
        Task ChangeUserDepartment(int id, int departmentId);
    }
}