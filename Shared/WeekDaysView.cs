using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Zebble
{
    partial class Calendar
    {
        public class WeekDaysView : Stack
        {
            DateTime startDate = DateTime.Today;
            List<TextView> WeekDayLabels;
            bool showWeekdays = true;
            string weekdayFormat = "ddd";
            DayOfWeek startDay = DayOfWeek.Sunday;

            public DateTime StartDate
            {
                get => startDate;
                set
                {
                    startDate = value.Date; Update();
                }
            }

            public DayOfWeek StartDay
            {
                get => startDay;
                set { startDay = value; Update(); }
            }

            public string WeekdayFormat
            {
                get => weekdayFormat;
                set { weekdayFormat = value; Update(); }
            }

            public bool ShowWeekdays
            {
                get => showWeekdays;
                set { showWeekdays = value; Update(); }
            }

            private WeekDaysView()
            {
                Direction = RepeatDirection.Horizontal; Id = "WeekdaysContainer";
                WeekDayLabels = new List<TextView>();
                Create();
                Update();
            }

            public static WeekDaysView CreateInstance() => new WeekDaysView();

            async Task Create()
            {
                for (var c = 0; c < CalendarHelpers.WEEK_DAYS; c++)
                {
                    var day = new TextView { CssClass = "weekday" };
                    WeekDayLabels.Add(day);
                    await AddAt(c, day);
                }
            }
            void Update()
            {
                if (!ShowWeekdays) return;
                var start = CalendarHelpers.GetCalendarStartDate(StartDate, StartDay);
                foreach (var label in WeekDayLabels)
                {
                    label.Text = start.ToString(WeekdayFormat);
                    start = start.AddDays(1);
                }
            }
        }
    }
}