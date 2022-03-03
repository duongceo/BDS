using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Core.Objects;
using HappyRE.Core.Entities.Model;
using HappyRE.Core.Entities.ViewModel;

namespace HappyRE.Core.Model
{
    public partial class HappyREContext
    {
        private void DBNullValue(SqlParameter[] items)
        {
            foreach (var p in items)
            {
                if (p.Value == null) p.Value = DBNull.Value;
            }
        }


        #region UserProfile
        public async Task<UserProfileListViewModel> msp_UserProfile_GetById(string id)
        {
            SqlParameter[] sqlParams = new SqlParameter[]{
                new SqlParameter(){ParameterName="id", DbType= DbType.String, Value = id}
            };

            this.DBNullValue(sqlParams);
            var storeParam = "@id";
            var results = await this.ExecuteStoreQueryAsync<UserProfileListViewModel>("[dbo].[msp_UserProfile_GetById] " + storeParam, sqlParams);
            return results.FirstOrDefault();
        }
        public async Task<int> msp_UserProfile_ChangeRole(string userId, string roleId)
        {
            SqlParameter[] sqlParams = new SqlParameter[]{
                new SqlParameter(){ParameterName="userId", DbType= DbType.String, Value = userId},
                new SqlParameter(){ParameterName="roleId", DbType= DbType.String, Value = roleId}
            };

            this.DBNullValue(sqlParams);
            var storeParam = "@userId,@roleId";
            var results = await this.ExecuteStoreCommandAsync("[dbo].[msp_UserProfile_ChangeRole] " + storeParam, sqlParams);
            return results;
        }
        #endregion
    }
}
