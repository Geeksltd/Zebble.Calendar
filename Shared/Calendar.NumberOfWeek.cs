namespace Zebble
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Threading.Tasks;

    public partial class Calendar
    {
        List<Grid> WeekNumbers = new List<Grid>(1);
        List<TextView> WeekNumberLabels = new List<TextView>(MAX_WEEK_IN_MONTH);
        CalendarWeekRule weekRule = CalendarWeekRule.FirstFourDayWeek;
        bool showNumberOfWeeks;

        public bool ShowNumberOfWeeks
        {
            get => showNumberOfWeeks;
            set { ShowHideElements().RunInParallel(); showNumberOfWeeks = value; }
        }

        public CalendarWeekRule WeekRule
        {
            get => weekRule;
            set
            {
                var start = GetCalendarStartDate(StartDate);
                for (var i = 0; i < Buttons.Count; i++)
                {
                    ChangeWeekNumbers(start, i);

                    start = start.AddDays(1);
                    if (i != 0 && (i + 1) % DAYS_IN_MONTH_VIEW == 0)
                        start = GetCalendarStartDate(start);
                }
                weekRule = value;
            }
        }

        protected void ChangeWeekNumbers(DateTime start, int index)
        {
            if (!ShowNumberOfWeeks) return;
            var weekNumber = CultureInfo.CurrentCulture.Calendar
                .GetWeekOfYear(start, CalendarWeekRule.FirstFourDayWeek, StartDay);
            WeekNumberLabels[index / WEEK_DAYS].Text = $"{weekNumber}";
        }

        protected async Task ShowHideElements()
        {
            if (MainCalendars.None()) return;

            await ContentView.ClearChildren();
            WeekDayLabels.Clear();

            for (var i = 0; i < MonthsToShow; i++)
            {
                var main = MainCalendars[i];

                if (ShowInBetweenMonthLabels && i > 0)
                {
                    var label = new TextView();

                    if (MonthTitleLabels == null)
                        MonthTitleLabels = new List<TextView>(MonthsToShow);

                    MonthTitleLabels.Add(label);
                    await ContentView.Add(label);
                }

                if (ShowNumberOfWeeks)
                {
                    main = new Grid { Direction = RepeatDirection.Horizontal };
                    await main.Add(WeekNumbers[i]);
                    await main.Add(MainCalendars[i]);
                }

                if (ShowWeekdays)
                {
                    var dayLabels = new Grid { Columns = WEEK_DAYS };

                    for (var c = 0; c < WEEK_DAYS; c++)
                    {
                        var day = new TextView { CssClass = "weekday" };
                        WeekDayLabels.Add(day);
                        await dayLabels.AddAt(c, day);
                    }

                    var stack = new Stack();
                    await stack.Add(dayLabels);
                    await stack.Add(main);
                    await ContentView.Add(stack);
                }
                else await ContentView.Add(main);
            }
        }
    }
}