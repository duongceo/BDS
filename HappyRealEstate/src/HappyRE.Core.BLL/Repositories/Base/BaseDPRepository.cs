using Dapper;
using HappyRE.Core.Entities;
using HappyRE.Core.Entities.Model;
using SQLinq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyRE.Core.BLL.Repositories
{
    public interface IBaseDPRepository<TEntity> where TEntity : new()
    {
        Task<TEntity> GetById(object id);
        Task<IEnumerable<TEntity>> GetWhere(SQLinq<TEntity> query);
        Task<int> Delete(object key);
        Task<int> Delete(TEntity entity, bool isPer = false);
        Task<int?> Insert(TEntity instance);
        Task<int> Update(TEntity instance);
        Task<Tuple<IEnumerable<T>, int>> Search<T>(BaseQuery query);
    }
    public abstract class BaseDPRepository<TEntity> : IBaseDPRepository<TEntity> where TEntity : new()
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["DBC_HappyRE"].ConnectionString;
        private IUow _uow = null;
        public IUow uow
        {
            get { if (_uow == null) _uow = ObjectFactory.GetInstance<IUow>(); return _uow; }
        }

        //private IConnectionFactory _connectionFactory = null;
        //public IConnectionFactory connectionFactory
        //{
        //    get { if (_connectionFactory == null) _connectionFactory = ObjectFactory.GetInstance<IConnectionFactory>(); return _connectionFactory; }
        //}

        protected IDbConnection _dbConnection { get; set; }
        public IDbConnection DbConnection
        {
            get { 
                return _dbConnection; 
            }
        }
        protected string _tableName { get; private set; }
        public BaseDPRepository(IUow uow)
        {
            if (uow == null)
            {
                throw new ArgumentNullException("uow is null");
            }
            _uow = uow;
            _dbConnection = _uow.dbContext.Database.Connection;
            _tableName = "HappyRE_" + typeof(TEntity).Name;
        }

        public async Task<TEntity> GetById(object id)
        {
            return await DbConnection.GetAsync<TEntity>(id);
        }

        public async Task<IEnumerable<TEntity>> GetWhere(SQLinq<TEntity> query)
        {
            var queryResult = query.ToSQL();

            // get the full SQL code
            var sqlCode = queryResult.ToQuery();

            // get the query parameters necessary to execute the above query
            var sqlParameters = queryResult.Parameters;

            return await DbConnection.QueryAsync<TEntity>(sqlCode, sqlParameters);
        }

        public async Task<TEntity> GetFirstOrDefault(SQLinq<TEntity> query)
        {
            var queryResult = query.ToSQL();

            // get the full SQL code
            var sqlCode = queryResult.ToQuery();

            // get the query parameters necessary to execute the above query
            var sqlParameters = queryResult.Parameters;

            return await DbConnection.QueryFirstOrDefaultAsync<TEntity>(sqlCode, sqlParameters);
        }
        public virtual void PreConditionUpdate(TEntity entity)
        {
            Type entityType = entity.GetType();
            var updatedUserFieldProperty = entityType.GetProperty("UpdatedBy");
            var updatedDateFieldProperty = entityType.GetProperty("UpdatedDate");
            if (updatedUserFieldProperty == null || updatedDateFieldProperty == null) return;

            string userName = System.Threading.Thread.CurrentPrincipal.Identity.IsAuthenticated ? System.Threading.Thread.CurrentPrincipal.Identity.Name : "System";

            var pros = entity.GetType().GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

            var pro_UpdateDate = pros.FirstOrDefault(x => x.Name == updatedDateFieldProperty.Name);
            if (pro_UpdateDate != null)
                pro_UpdateDate.SetValue(entity, DateTime.Now);

            var pro_UpdateBy = pros.FirstOrDefault(x => x.Name == updatedUserFieldProperty.Name);
            if (pro_UpdateBy != null)
                pro_UpdateBy.SetValue(entity, userName);
        }
        public virtual void PreConditionAdd(TEntity entity)
        {
            Type entityType = entity.GetType();
            var updatedUserFieldProperty = entityType.GetProperty("CreatedBy");
            var createdDateFieldProperty = entityType.GetProperty("CreatedDate");
            var updatedDateFieldProperty = entityType.GetProperty("UpdatedDate");
            if (updatedUserFieldProperty == null || updatedDateFieldProperty == null || createdDateFieldProperty==null) return;

            string userName = System.Threading.Thread.CurrentPrincipal.Identity.IsAuthenticated ? System.Threading.Thread.CurrentPrincipal.Identity.Name : "System";

            var pros = entity.GetType().GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

            var pro_CreateDate = pros.FirstOrDefault(x => x.Name == createdDateFieldProperty.Name);
            if (pro_CreateDate != null)
                pro_CreateDate.SetValue(entity, DateTime.Now);

            var pro_UpdateDate = pros.FirstOrDefault(x => x.Name == updatedDateFieldProperty.Name);
            if (pro_UpdateDate != null)
                pro_UpdateDate.SetValue(entity, DateTime.Now);

            var pro_UpdateBy = pros.FirstOrDefault(x => x.Name == updatedUserFieldProperty.Name);
            if (pro_UpdateBy != null)
                pro_UpdateBy.SetValue(entity, userName);
        }
        public async Task<int?> Insert(TEntity instance)
        {
            PreConditionAdd(instance);
            return await DbConnection.InsertAsync(instance);
        }
        public async Task<int> Delete(object key)
        {
            return await DbConnection.DeleteAsync(key);
        }

        public virtual async Task DeleteCheck(TEntity entity)
        {

        }
        public virtual async Task DeleteAfter(TEntity entity)
        {

        }
        public async Task<int> Delete(TEntity entity, bool isPer=false)
        {
           await DeleteCheck(entity);
            if (isPer==true) return await DbConnection.DeleteAsync(entity);
            else
            {
                Type entityType = entity.GetType();
                var idField = entityType.GetProperty("Id");
                var pros = entity.GetType().GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

                var id_prop = pros.FirstOrDefault(x => x.Name == idField.Name);
                var id = id_prop.GetValue(entity);

               var res= await DbConnection.ExecuteScalarAsync<int>($"update {entityType.Name} set Deleted = 1 where Id=@id", new { id }, commandType: CommandType.Text);
               await DeleteAfter(entity);
               return res;
            }
        }

        public async Task<int> Update(TEntity instance)
        {
            PreConditionUpdate(instance);
            return await DbConnection.UpdateAsync(instance);
        }

        public async Task<IEnumerable<TEntity>> Query(string str, object param = null, CommandType? commandType = null)
        {
            return await DbConnection.QueryAsync<TEntity>(str, param, commandType: commandType);
        }

        public async Task<IEnumerable<T>> Query<T>(string str, object param = null, CommandType? commandType = null)
        {
            return await DbConnection.QueryAsync<T>(str, param, commandType: commandType);
        }

        public async Task<bool> IsExistsName<T>(string where, object param = null)
        {
            var u= await DbConnection.RecordCountAsync<T>(where, param, null, 0);
            return u > 0;
        }

        public async Task<Tuple<IEnumerable<T>,int>> GetListPaged<T>(int page, int limit, string where,string orderBy="id", object param = null)
        {
            var list = await DbConnection.GetListPagedAsync<T>(page, limit, where, orderBy, param);
            var count = await DbConnection.RecordCountAsync<T>(where, param,null,0);
            return new Tuple<IEnumerable<T>, int>(list, count);
        }

        public async Task<int> Execute(string str, object param = null, CommandType? commandType = null)
        {
            return await DbConnection.ExecuteAsync(str, param, commandType: commandType);
        }

        public async Task<T> ExecuteScalar<T>(string str, object param = null, CommandType? commandType = null)
        {
            return await DbConnection.ExecuteScalarAsync<T>(str, param, commandType: commandType);
        }

        public T ExecuteScalarNonAsync<T>(string str, object param = null, CommandType? commandType = null)
        {
            return DbConnection.ExecuteScalar<T>(str, param, commandType: commandType);
        }

        public IEnumerable<T> QueryNonAsync<T>(string str, object param = null, CommandType? commandType = null)
        {
            return DbConnection.Query<T>(str, param, commandType: commandType);
        }

        public async Task<Tuple<IEnumerable<T>, int>> Search<T>(BaseQuery query)
        {
            string conditions = "where Deleted=0 ";
            if (string.IsNullOrEmpty(query.Keyword) == false) conditions += "and name like @keyword";
            return await this.GetListPaged<T>(query.Page, query.Limit, conditions, "id", new { keyword = query.KeywordLike });
        }

        #region Log
        public virtual void LogActivity(int? tableKeyId, string content, string action = "add",bool isTracking=true)
        {
            if (isTracking == true)
            {
                //for tracking
                string userName = System.Threading.Thread.CurrentPrincipal.Identity.IsAuthenticated ? System.Threading.Thread.CurrentPrincipal.Identity.Name : "System";
                Hangfire.BackgroundJob.Enqueue<IHistoryLogRepository>(x => x.AddTrackingLog(new HistoryLog()
                {
                    TableName = _tableName,
                    TableKeyId = tableKeyId,
                    Action = action,
                    Contents = content,
                    CreatedBy= userName
                }));
            }
            else
            {
                //for comment
                Hangfire.BackgroundJob.Enqueue<IHistoryLogRepository>(x => x.IU(new HistoryLog()
                {
                    TableName = _tableName,
                    TableKeyId = tableKeyId,
                    Action = action,
                    Contents = content
                }));
            }
        }
        #endregion
    }
}
