
using System;
using System.Web.Mvc;

namespace CF.Infracstructures
{
    public static class ToResult
    {
        public static JsonResult ToJsonResult(this int result)
        {

            return new JsonResult(){Data = new {}};
        }

        public static Tuple<DateTime, DateTime> ConvertRange(string key)
        {
            DateTime from = DateTime.Today;
            DateTime to = DateTime.Today.AddDays(1);
            switch (key)
            {
                case "today":
                    break;
                case "yesterday":
                    from = DateTime.Today.AddDays(-1);
                    to = DateTime.Today;
                    break;
                case "thisweek":
                    from = DateTime.Today.AddDays(DayOfWeek.Sunday - DateTime.Today.DayOfWeek);
                    break;
                case "thismonth":
                    from = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                    to = from.AddMonths(1);
                    break;
                case "premonth":
                    from = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(-1);
                    to = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                    break;
            }
            return new Tuple<DateTime, DateTime>(from, to);
        }
    }
}