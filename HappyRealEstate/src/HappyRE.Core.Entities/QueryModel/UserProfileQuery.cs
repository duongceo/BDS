using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyRE.Core.Entities
{
    public class UserProfileQuery: BaseQuery
    {
        public int? LevelId_Filter { get; set; }
        public int? DepartmentId_Filter { get; set; }
        public int? RoleGroupId_Filter { get; set; }
        public int? UserStatus_Filter { get; set; }
    }
}
