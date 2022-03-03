
using Dapper;
using HappyRE.Core.Entities;
using HappyRE.Core.Entities.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace HappyRE.Core.BLL.Repositories
{
    public class HistoryLogRepository : BaseDPRepository<HistoryLog>, IHistoryLogRepository
    {
        public HistoryLogRepository(IUow uow)
            : base(uow)
        {
        }

        public async Task<Tuple<IEnumerable<HistoryLog>, int>> SearchTrackingLog(HistoryLogQuery query)
        {
            var p = new DynamicParameters();

            p.Add("total", dbType: DbType.Int32, direction: ParameterDirection.Output);
            p.Add("limit", query.Limit);
            p.Add("page", query.Page);
            p.Add("keyword", query.Keyword);
            p.Add("tableName", query.TableName);
            p.Add("tableKeyId", query.TableKeyId);
            p.Add("action", query.Action);
            p.Add("createdBy", query.CreatedBy);
            var res = await this.Query<HistoryLog>("msp_Tracking_HistoryLogSearch", p, System.Data.CommandType.StoredProcedure);
            var total = p.Get<int>("total");
            return new Tuple<IEnumerable<HistoryLog>, int>(res, total);
        }

        public async Task<Tuple<IEnumerable<HistoryLog>, int>> SearchTrackingLogUserDetail(HistoryLogQuery query)
        {
            var p = new DynamicParameters();

            p.Add("total", dbType: DbType.Int32, direction: ParameterDirection.Output);
            p.Add("limit", query.Limit);
            p.Add("page", query.Page);
            p.Add("keyword", query.Keyword);
            p.Add("tableName", query.TableName);
            p.Add("tableKeyId", query.TableKeyId);
            p.Add("action", query.Action);
            p.Add("createdBy", query.CreatedBy);
            var res = await this.Query<HistoryLog>("msp_Tracking_HistoryLogSearchUserDetail", p, System.Data.CommandType.StoredProcedure);
            var total = p.Get<int>("total");
            return new Tuple<IEnumerable<HistoryLog>, int>(res, total);
        }

        public async Task<Tuple<IEnumerable<HistoryLog>, int>> Search(HistoryLogQuery query)
        {
            var p = new DynamicParameters();
            p.Add("total", dbType: DbType.Int32, direction: ParameterDirection.Output);
            p.Add("limit", query.Limit);
            p.Add("page", query.Page);
            p.Add("keyword", query.Keyword);
            p.Add("tableName", query.TableName);
            p.Add("tableKeyId", query.TableKeyId);
            var res = await this.Query<HistoryLog>("msp_HistoryLog_Search", p, System.Data.CommandType.StoredProcedure);
            var total = p.Get<int>("total");
            return new Tuple<IEnumerable<HistoryLog>, int>(res, total);
        }

        public async Task<int?> IU(HistoryLog obj)
        {
            var m = await GetById(obj.Id);
            if (m == null)
            {
                return await this.Insert(obj);
            }
            else
            {
                return await this.Update(m);
            }
        }

        public async Task<int?> AddTrackingLog(HistoryLog obj)
        {
           return await this.ExecuteScalar<int>("msp_Tracking_AddLog", new { obj.TableName, obj.TableKeyId, obj.Action, obj.Contents, obj.Type, obj.CreatedBy }, System.Data.CommandType.StoredProcedure);
        }


        public async Task<Tuple<IEnumerable<TrackChange>, int>> SearchTrack(TrackChangeQuery query)
        {
            var p = new DynamicParameters();
            p.Add("total", dbType: DbType.Int32, direction: ParameterDirection.Output);
            p.Add("limit", query.Limit);
            p.Add("page", query.Page);
            p.Add("keyword", query.Keyword);
            p.Add("tableId", query.TableId);
            p.Add("tableKeyId", query.TableKeyId);
            var res = await this.Query<TrackChange>("msp_TrackChange_Search", p, System.Data.CommandType.StoredProcedure);
            var total = p.Get<int>("total");
            return new Tuple<IEnumerable<TrackChange>, int>(res, total);
        }
    }
}
