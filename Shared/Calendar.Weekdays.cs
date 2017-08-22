using System;
using System.Collections.Generic;

namespace Zebble
{
    public partial class Calendar
    {
        List<TextView> WeekDayLabels;
        bool showWeekdays = true;
        string weekdayFormat = "ddd";

        public string WeekdayFormat
        {
            get => weekdayFormat;
            set { weekdayFormat = value; UpdateWeekdays(); }
        }

        public bool ShowWeekdays
        {
            get => showWeekdays;
            set { ShowHideElements().RunInParallel(); showWeekdays = value; }
        }

        void UpdateWeekdays()
        {
            if (!ShowWeekdays) return;
            var start = GetCalendarStartDate(StartDate);
            foreach (var label in WeekDayLabels)
            {
                label.Text = start.ToString(WeekdayFormat);
                start = start.AddDays(1);
            }
        }
    }
}
