using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyRE.Core.Utils.Helpers
{
    public static class DateHelper
    {
        public static string ToFriendlyDate(this DateTime d)
        {
            // 1.
            // Get time span elapsed since the date.
            TimeSpan s = DateTime.Now.Subtract(d);

            // 2.
            // Get total number of days elapsed.
            int dayDiff = (int)s.TotalDays;

            // 3.
            // Get total number of seconds elapsed.
            int secDiff = (int)s.TotalSeconds;

            // 4.
            // Don't allow out of range values.
            if (dayDiff < 0)
            {
                return dayDiff == -1 ? "ngày mai" : dayDiff == -2 ? "ngày mốt" : (dayDiff * -1).ToString() + " ngày sau";
            }

            // 5.                       
            // Handle same-day times.
            if (dayDiff == 0)
            {
                // A.
                // Less than one minute ago.
                if (secDiff < 60)
                {
                    return "vừa mới đây";
                }
                // B.
                // Less than 2 minutes ago.
                if (secDiff < 120)
                {
                    return "1 phút trước";
                }
                // C.
                // Less than one hour ago.
                if (secDiff < 3600)
                {
                    return string.Format("{0} phút trước",
                        Math.Floor((double)secDiff / 60));
                }
                // D.
                // Less than 2 hours ago.
                if (secDiff < 7200)
                {
                    return "1 tiếng trước";
                }
                // E.
                // Less than one day ago.
                if (secDiff < 86400)
                {
                    return string.Format("{0} tiếng trước",
                        Math.Floor((double)secDiff / 3600));
                }
            }

            // 6.
            // Handle previous days.
            if (dayDiff == 1)
            {
                return "hôm qua";
            }
            if (dayDiff < 7)
            {
                return string.Format("{0} ngày trước",
                    dayDiff);
            }
            if (dayDiff < 31)
            {
                return string.Format("{0} tuần trước",
                    Math.Ceiling((double)dayDiff / 7));
            }
            if (dayDiff < 365)
            {
                return string.Format("{0} tháng trước",
                        Math.Floor((double)dayDiff / 31));
            }
            if (dayDiff > 365)
            {
                return string.Format("{0} năm trước",
                        Math.Floor((double)dayDiff / 365));
            }

            return null;
        }

        public static long ConvertDateTimeToUnix(DateTime d)
        {
            return (long)(d.Subtract(new DateTime(1970, 1, 1, 0, 0, 0))).TotalSeconds;
        }
    }
}
