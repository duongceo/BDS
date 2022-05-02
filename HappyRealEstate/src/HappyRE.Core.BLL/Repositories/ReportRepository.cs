using HappyRE.Core.Utils;
using HappyRE.Core.Entities;
using HappyRE.Core.Entities.Model;
using HappyRE.Core.Entities.ViewModel;
using HappyRE.FileServiceProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Hangfire;
using HappyRE.Core.Entities.QueryModel;

namespace HappyRE.Core.BLL.Repositories
{
    public class ReportRepository : BaseDPRepository<File>, IReportRepository
    {
        public ReportRepository(IUow uow) : base(uow) { }

        public async Task<ReportSummaryViewModel> Summary(ReportQuery query)
        {
            var res = await base.Query<ReportSummaryViewModel>("msp_Report_Summary", new { fromDate = query.FromDate, toDate = query.ToDate }, System.Data.CommandType.StoredProcedure);
            return res.FirstOrDefault();
        }

        public async Task<IEnumerable<ReportTopUserViewModel>> TopUserHighPerformance(ReportQuery query)
        {
            var res = await base.Query<ReportTopUserViewModel>("msp_Report_TopUser_HighPerformance", new { fromDate = query.FromDate, toDate = query.ToDate }, System.Data.CommandType.StoredProcedure);
            return res;
        }

        public async Task<IEnumerable<ReportTopUserViewModel>> TopUserLowPerformance(ReportQuery query)
        {
            var res = await base.Query<ReportTopUserViewModel>("msp_Report_TopUser_LowPerformance", new { fromDate = query.FromDate, toDate = query.ToDate }, System.Data.CommandType.StoredProcedure);
            return res;
        }

        public async Task<IEnumerable<ReportTopUserViewModel>> TopUserPropertyAdd(ReportQuery query)
        {
            var res = await base.Query<ReportTopUserViewModel>("msp_Report_TopUser_PropertyAdd", new { fromDate = query.FromDate, toDate = query.ToDate }, System.Data.CommandType.StoredProcedure);
            return res;
        }

        public async Task<IEnumerable<ReportTopUserViewModel>> TopUserPropertyViewMobile(ReportQuery query)
        {
            var res = await base.Query<ReportTopUserViewModel>("msp_Report_TopUser_PropertyViewMobile", new { fromDate = query.FromDate, toDate = query.ToDate }, System.Data.CommandType.StoredProcedure);
            return res;
        }

        public async Task<IEnumerable<ReportDailyViewModel>> PropertyDaily(ReportPropertyDailyQuery query)
        {
            var res = await base.Query<ReportDailyViewModel>("msp_Report_Property_Daily", new { fromDate = query.FromDate, toDate = query.ToDate, query.TypeId, query.StatusId, query.IsChecked, query.UserName, query.Unit }, System.Data.CommandType.StoredProcedure);
            return res;
        }

        public async Task<IEnumerable<ReportDailyViewModel>> SaleOrderDaily(ReportSaleOrderDailyQuery query)
        {
            var res = await base.Query<ReportDailyViewModel>("msp_Report_SaleOrder_Daily", new { fromDate = query.FromDate, toDate = query.ToDate, query.OwnerTargetId, query.CustomerTargetId, query.PostedBy, query.SellBy, query.Unit }, System.Data.CommandType.StoredProcedure);
            return res;
        }
    }
}
