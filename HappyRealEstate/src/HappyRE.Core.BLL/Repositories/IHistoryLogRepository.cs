using HappyRE.Core.Entities;
using HappyRE.Core.Entities.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HappyRE.Core.BLL.Repositories
{
    public interface IHistoryLogRepository : IBaseDPRepository<HistoryLog>
    {
        Task<int?> IU(HistoryLog obj);
        Task<int?> AddTrackingLog(HistoryLog obj);
        Task<Tuple<IEnumerable<HistoryLog>, int>> Search(HistoryLogQuery query);
        Task<Tuple<IEnumerable<HistoryLog>, int>> SearchTrackingLog(HistoryLogQuery query);
        Task<Tuple<IEnumerable<HistoryLog>, int>> SearchTrackingLogUserDetail(HistoryLogQuery query);
        Task<Tuple<IEnumerable<TrackChange>, int>> SearchTrack(TrackChangeQuery query);
    }
}