namespace Zebble
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public partial class Calendar : View
    {
        Stack MainView;
        Stack ItemsContainer;

        HeaderView Header;
        DaysView Days;
        WeekDaysView WeekDays;
        YearsView Years;

        bool limitSelectionToRange;

        DateTime startDate;
        DayOfWeek startDay;

        Stack TemporatyView;

        public CalendarScope Scope { get; protected set; }

        public Calendar()
        {
            MainView = new Stack();
            ItemsContainer = new Stack { Id = "ItemsContainer" };

            Header = new HeaderView();
            Header.PreviousTapped.HandleWith(Header_PreviousTapped);
            Header.NextTapped.HandleWith(Header_NextTapped);
            Header.TitleTapped.Handle(() => ChangeScope(nextScope: true));

            Days = new DaysView();
            WeekDays = new WeekDaysView();

            startDate = DateTime.Today;
            startDay = DayOfWeek.Sunday;

            Scope = CalendarScope.Days;
        }

        public override async Task OnInitializing()
        {
            await base.OnInitializing();

            await MainView.Add(Header);

            await MainView.Add(ItemsContainer);

            await ItemsContainer.Add(WeekDays);

            await ItemsContainer.Add(Days);
        }

        public bool LimitSelectionToRange
        {
            get => limitSelectionToRange;
            set { limitSelectionToRange = value; ChangeCalendar(); }
        }

        public DateTime? MinDate
        {
            get => Days.MinDate;
            set => Days.MinDate = value;
        }

        public DateTime? MaxDate
        {
            get => Days.MaxDate;
            set => Days.MaxDate = value;
        }

        public DateTime StartDate
        {
            get => startDate;
            set { startDate = value.Date; ChangeCalendar(); }
        }

        public DayOfWeek StartDay
        {
            get => startDay;
            set { startDay = value; ChangeCalendar(); }
        }

        public int MonthsToShow
        {
            get => Days.MonthsToShow;
            set => Days.MonthsToShow = value;
        }

        public List<SpecialDate> SpecialDates
        {
            get => Days.SpecialDates;
            set => Days.SpecialDates = value;
        }

        public override async Task OnPreRender()
        {
            await base.OnPreRender();
            await ChangeCalendar();
        }

        async Task ChangeCalendar()
        {
            Header.TitleText = StartDate.ToString("MMM yyyy");

            var start = CalendarHelpers.GetCalendarStartDate(StartDate, StartDay);

            Days.StartDate = StartDate;

            if (LimitSelectionToRange)
            {
                Header.PreviousEnabled = !(MinDate.HasValue && CalendarHelpers.GetCalendarStartDate(StartDate, StartDay) < MinDate);
                Header.NextEnabled = !(MaxDate.HasValue && start > MaxDate);
            }
            await Add(MainView);
        }

        bool lockScope;
        public bool LockScope
        {
            get => lockScope;
            set
            {
                lockScope = value;
                if (value) Header.TitleTapped.Handle(() => ChangeScope(nextScope: true));
            }
        }

        void Header_NextTapped()
        {
            if (Scope == CalendarScope.Years)
                startDate = Years.NextPage();
            else
                startDate = Days.NextPage();
            
            Header.TitleText = StartDate.ToString("MMM yyyy");
        }

        void Header_PreviousTapped()
        {
            if (Scope == CalendarScope.Years)
                startDate = Years.PreviousPage();
            else
                startDate = Days.PreviousPage();
            Header.TitleText = StartDate.ToString("MMM yyyy");
        }

        ///
        /// MonthYearView
        async Task ChangeScope(bool nextScope)
        {
            if (TemporatyView == null)
            {
                await CloneAndClearContentView();
            }
            switch (Scope)
            {
                case CalendarScope.Days:
                    {
                        if (nextScope)
                            await GoToMonths();
                        else
                            await GoToYears();
                        break;
                    }
                case CalendarScope.Months:
                    {
                        if (nextScope)
                            await GoToYears();
                        else
                            await GoToDays();
                        break;
                    }
                case CalendarScope.Years:
                    {
                        if (nextScope)
                            await GoToDays();
                        else
                            await GoToMonths();
                        break;
                    }
                default: await GoToDays(); break;
            }
        }

        async Task CloneAndClearContentView()
        {
            if (TemporatyView == null)
                TemporatyView = new Stack().Absolute().Hide();
            var children = ItemsContainer.AllChildren;

            if (children.Any(x => x.Native != null))
                await Root.Add(TemporatyView, awaitNative: true);

            foreach (var child in children)
                await child.MoveTo(TemporatyView);
        }

        async Task ReAddToContentView()
        {
            await ItemsContainer.ClearChildren();
            var children = TemporatyView.AllChildren;
            foreach (var child in children)
                await child.MoveTo(ItemsContainer);
            TemporatyView = null;
        }

        async Task GoToDays()
        {
            Scope = CalendarScope.Days;
            Header.PreviousVisible = Header.NextVisible = true;
            await ItemsContainer.ClearChildren();
            await ReAddToContentView();
            await ChangeCalendar();
        }

        async Task GoToMonths()
        {
            var months = new MonthsView(StartDate);
            months.MonthTapped.Handle(async args =>
            {
                if (!LockScope)
                {
                    StartDate = args;
                    await ChangeScope(nextScope: false);
                }
            });
            await ItemsContainer.ClearChildren();
            await ItemsContainer.Add(months);

            Scope = CalendarScope.Months;
            Header.PreviousVisible = Header.NextVisible = false;
        }

        async Task GoToYears()
        {
            Years = new YearsView(StartDate);
            Years.YearTapped.Handle(async args =>
            {
                if (!LockScope)
                {
                    StartDate = args;
                    await ChangeScope(nextScope: false);
                }
            });
            await ItemsContainer.ClearChildren();
            await ItemsContainer.Add(Years);
            Scope = CalendarScope.Years;
            Header.PreviousVisible = true;
            Header.NextVisible = true;
        }
    }
}