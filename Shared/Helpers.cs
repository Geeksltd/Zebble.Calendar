using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Zebble
{
    public static class CalendarHelpers
    {
        public const int WEEK_DAYS = 7;
        public const int MAX_WEEK_IN_MONTH = 6;
        public const int DAYS_IN_MONTH_VIEW = WEEK_DAYS * MAX_WEEK_IN_MONTH;

        public static DateTime GetCalendarStartDate(DateTime date, DayOfWeek startDay)
        {
            var start = date;
            var beginOfMonth = start.Day == 1;
            while (!beginOfMonth || start.DayOfWeek != startDay)
            {
                start = start.AddDays(-1);
                beginOfMonth |= start.Day == 1;
            }
            return start;
        }

    }
}
