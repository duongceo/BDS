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
    public class DepartmentRepository : BaseDPRepository<Department>, IDepartmentRepository
    {
        public DepartmentRepository(IUow uow)
            : base(uow)
        {
        }

        public IEnumerable<KeyValueModel> GetAll()
        {
            return this.QueryNonAsync<KeyValueModel>("select Id, Name from Department (nolock) where Deleted=0", new { }, CommandType.Text);
        }

        public override async Task DeleteCheck(Department obj)
        {
            var c= await this.ExecuteScalar<int>("select count(*) from UserProfile (nolock) where Deleted=0 and DepartmentId=@departmentId", new { departmentId= obj.Id }, CommandType.Text);
            if (c > 0) throw new BusinessException($"Không thể xóa phòng ban này vì đang có {c} nhân viên!");
        }
        public async Task<Tuple<IEnumerable<DepartmentListViewModel>, int>> Search(BaseQuery query)
        {
            var p = new DynamicParameters();

            p.Add("total", dbType: DbType.Int32, direction: ParameterDirection.Output);
            p.Add("limit", query.Limit);
            p.Add("page", query.Page);
            p.Add("keyword", query.Keyword);
            var res = await this.Query<DepartmentListViewModel>("msp_Department_Search", p, System.Data.CommandType.StoredProcedure);
            var total = p.Get<int>("total");
            return new Tuple<IEnumerable<DepartmentListViewModel>, int>(res, total);
        }
        public async Task<int?> IU(Department obj)
        {
            var m = await this.GetById(obj.Id);
            if (m == null)
            {
                return await this.Insert(obj);
            }
            else
            {
                m.Code = obj.Code;
                m.Name = obj.Name;
                m.StartDate = obj.StartDate;
                m.Phone = obj.Phone;
                m.Note = obj.Note;
                await this.Update(m);
                return m.Id;
            }
        }

        public async Task<int?> UpdateManager(Department obj)
        {
            var m = await this.GetById(obj.Id);
            if (m != null)
            {
                var query = @"update Department
                            set ManagerId=null
                            where ManagerId=@ManagerId";
                await this.ExecuteScalar<int>(query, new { ManagerId = obj.ManagerId }, CommandType.Text);

                m.ManagerId = obj.ManagerId;
                var r= await this.Update(m);
                if (m.ManagerId.HasValue)
                {
                    await uow.UserProfile.ChangeUserDepartment(m.ManagerId.Value, m.Id);
                }
                return r;
            }
            else return 0;
        }

        public async Task<IEnumerable<DepartmentListViewModel>> Export(BaseQuery query)
        {
            query.Page = 1;
            query.Limit = 1000000;
            var res = await Search(query);
            return res.Item1;
        }
    }
}
