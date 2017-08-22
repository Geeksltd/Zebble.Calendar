namespace Zebble
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public partial class Calendar : View
    {
        const int WEEK_DAYS = 7;
        const int MAX_WEEK_IN_MONTH = 6;
        const int DAYS_IN_MONTH_VIEW = WEEK_DAYS * MAX_WEEK_IN_MONTH;

        List<ItemButton> Buttons;
        List<Grid> MainCalendars;
        List<TextView> MonthTitleLabels;
        Stack MainView, ContentView;

        bool limitSelectionToRange, showInBetweenMonthLabels = true;
        int monthsToShow = 1;
        DateTime? minDate, maxDate;
        DateTime startDate = DateTime.Today;
        DayOfWeek startDay = DayOfWeek.Sunday;
        List<SpecialDate> specialDates = new List<SpecialDate>();

        public Calendar()
        {
            TitleLeftArrow = new ItemButton { Text = "❰" };
            TitleLabel = new TextView();
            TitleRightArrow = new ItemButton { Text = "❱" };
            MonthNavigationLayout = new Stack { Direction = RepeatDirection.Horizontal };

            MonthNavigationLayout.Add(TitleLeftArrow);
            MonthNavigationLayout.Add(TitleLabel);
            MonthNavigationLayout.Add(TitleRightArrow);

            MainView = new Stack();
            MainView.Add(MonthNavigationLayout);
            MainView.Add(ContentView = new Stack());

            TitleLeftArrow.Tapped.HandleWith(LeftArrowTapped);
            TitleRightArrow.Tapped.HandleWith(RightArrowTapped);

            TitleLabel.Tapped.Handle(() => NextMonthYearView());

            WeekDayLabels = new List<TextView>(WEEK_DAYS);

            Buttons = new List<ItemButton>(DAYS_IN_MONTH_VIEW);
            MainCalendars = new List<Grid>(1);

            Scope = CalendarScope.Days;
        }

        public bool LimitSelectionToRange
        {
            get => limitSelectionToRange;
            set { ChangeCalendar(CalandarChanges.MaxMin); limitSelectionToRange = value; }
        }

        public DateTime? MinDate
        {
            get => minDate;
            set { ChangeCalendar(CalandarChanges.MaxMin); minDate = value; }
        }

        public DateTime? MaxDate
        {
            get => maxDate;
            set { ChangeCalendar(CalandarChanges.MaxMin); maxDate = value; }
        }

        public DateTime StartDate
        {
            get => startDate;
            set { ChangeCalendar(CalandarChanges.StartDate); startDate = value.Date; }
        }

        public DayOfWeek StartDay
        {
            get => startDay;
            set { ChangeCalendar(CalandarChanges.StartDay); startDay = value; }
        }

        public int MonthsToShow
        {
            get { return monthsToShow; }
            set { ChangeCalendar(CalandarChanges.All); monthsToShow = value; }
        }

        public bool ShowInBetweenMonthLabels
        {
            get { return showInBetweenMonthLabels; }
            set
            {
                ChangeCalendar(CalandarChanges.All);
                showInBetweenMonthLabels = value;
            }
        }

        public List<SpecialDate> SpecialDates
        {
            get => specialDates;
            set { ChangeCalendar(CalandarChanges.MaxMin); specialDates = value; }
        }

        public DateTime GetCalendarStartDate(DateTime date)
        {
            var start = date;
            var beginOfMonth = start.Day == 1;
            while (!beginOfMonth || start.DayOfWeek != StartDay)
            {
                start = start.AddDays(-1);
                beginOfMonth |= start.Day == 1;
            }
            return start;
        }

        public override async Task OnPreRender()
        {
            await FillWindows();
            await base.OnPreRender();
            ChangeCalendar(CalandarChanges.All);
        }

        protected Task Fill() => FillWindows();

        protected async Task FillWindows()
        {
            await CreateWeeknumbers();
            await CreateButtons();
            await ShowHideElements();
        }

        protected async Task CreateWeeknumbers()
        {
            WeekNumberLabels.Clear();
            WeekNumbers.Clear();
            if (!ShowNumberOfWeeks) return;

            for (var i = 0; i < MonthsToShow; i++)
            {
                var weekNumbers = new Grid { Columns = 1 };

                for (var row = 0; row < MAX_WEEK_IN_MONTH; row++)
                {
                    var txt = new TextView();
                    WeekNumberLabels.Add(txt);
                    await weekNumbers.AddAt(row, txt);
                }
                WeekNumbers.Add(weekNumbers);
            }
        }

        protected async Task CreateButtons()
        {
            Buttons.Clear();
            MainCalendars.Clear();
            for (var i = 0; i < MonthsToShow; i++)
            {
                var mainCalendar = new Grid { Columns = WEEK_DAYS };

                for (var row = 0; row < MAX_WEEK_IN_MONTH; row++)
                {
                    for (var column = 0; column < WEEK_DAYS; column++)
                    {
                        var button = new ItemButton();

                        Buttons.Add(button);
                        var lastButton = Buttons.Last();
                        lastButton.Tapped.Handle(ItemButtonTapped);

                        await mainCalendar.Add(button);
                    }
                }
                MainCalendars.Add(mainCalendar);
                MainCalendars.ForEach(async a => await a.EnsureFullColumns());
            }
        }

        public void ForceRedraw() => ChangeCalendar(CalandarChanges.All);

        protected void ChangeCalendar(CalandarChanges changes)
        {
            if (changes.HasFlag(CalandarChanges.StartDate))
            {
                TitleLabel.Text = StartDate.ToString("MMM yyyy");
                if (MonthTitleLabels != null)
                {
                    var tls = StartDate.AddMonths(1);
                    foreach (var tl in MonthTitleLabels)
                    {
                        tl.Text = tls.ToString();
                        tls = tls.AddMonths(1);
                    }
                }
            }

            var start = GetCalendarStartDate(StartDate);
            var beginOfMonth = false;
            var endOfMonth = false;
            for (var i = 0; i < Buttons.Count; i++)
            {
                endOfMonth |= beginOfMonth && start.Day == 1;
                beginOfMonth |= start.Day == 1;

                if (i < WeekDayLabels.Count && ShowWeekdays && changes.HasFlag(CalandarChanges.StartDay))
                    WeekDayLabels[i].Text = start.ToString(WeekdayFormat);

                ChangeWeekNumbers(start, i);

                Buttons[i].Text = $"{start.Day}";
                Buttons[i].Date = start;
                Buttons[i].OutOfMonth = !(beginOfMonth && !endOfMonth);
                Buttons[i].Enabled = MonthsToShow == 1 || !Buttons[i].OutOfMonth;

                var specialDate = SpecialDates?.FirstOrDefault(s => s.Date == start);

                Unselect(Buttons[i]);

                if (start < MinDate || start > MaxDate) Buttons[i].SetDisabled();
                else if (Buttons[i].Enabled && SelectedDates.Contains(start))
                    Buttons[i].Select();
                else if (specialDate != null) Buttons[i].Enabled = specialDate.Selectable;

                start = start.AddDays(1);
                if (i != 0)
                {
                    if ((i + 1) % DAYS_IN_MONTH_VIEW == 0)
                    {
                        beginOfMonth = false;
                        endOfMonth = false;
                        start = GetCalendarStartDate(start);
                    }
                }
            }

            if (LimitSelectionToRange)
            {
                TitleLeftArrow.Enabled = !(MinDate.HasValue && GetCalendarStartDate(StartDate) < MinDate);
                TitleRightArrow.Enabled = !(MaxDate.HasValue && start > MaxDate);
            }

            Add(MainView);
        }

        protected void Unselect(ItemButton button)
        {
            button.Selected = false;
            button.Enabled = MonthsToShow == 1 || !button.OutOfMonth;
        }

        Task ItemButtonTapped(TouchEventArgs args)
        {
            var item = args.View as ItemButton;

            var selectedDate = item.Date;
            if (SelectedDate.HasValue && selectedDate.HasValue && SelectedDate == selectedDate)
            {
                ChangeSelectedDate(selectedDate);
                SelectedDate = null;
            }
            else SelectedDate = selectedDate;

            return Task.CompletedTask;
        }
    }
}