using HappyRE.Core.Entities.Model;
using HappyRE.Core.Entities.QueryModel;
using HappyRE.Core.Entities.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HappyRE.Core.BLL.Repositories
{
    public interface IReportRepository:IBaseDPRepository<File>
    {
        Task<IEnumerable<ReportDailyViewModel>> PropertyDaily(ReportPropertyDailyQuery query);
        Task<IEnumerable<ReportDailyViewModel>> SaleOrderDaily(ReportSaleOrderDailyQuery query);
        Task<ReportSummaryViewModel> Summary(ReportQuery query);
        Task<IEnumerable<ReportTopUserViewModel>> TopUserHighPerformance(ReportQuery query);
        Task<IEnumerable<ReportTopUserViewModel>> TopUserLowPerformance(ReportQuery query);
        Task<IEnumerable<ReportTopUserViewModel>> TopUserPropertyAdd(ReportQuery query);
        Task<IEnumerable<ReportTopUserViewModel>> TopUserPropertyViewMobile(ReportQuery query);
    }
}