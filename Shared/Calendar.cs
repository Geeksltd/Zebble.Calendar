namespace Zebble
{
    using System;
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

        bool lockScope;

        DateTime startDate;
        DayOfWeek startDay;

        Stack TemporatyView;

        public CalendarScope Scope { get; protected set; }

        public Calendar()
        {
            startDate = DateTime.Today;
            startDay = DayOfWeek.Sunday;

            Scope = CalendarScope.Days;
            MainView = new Stack();
            ItemsContainer = new Stack { Id = "ItemsContainer" };

            Header = new HeaderView();
            Header.PreviousTapped.HandleWith(Header_PreviousTapped);
            Header.NextTapped.HandleWith(Header_NextTapped);
            Header.TitleTapped.Handle(() => ChangeScope(nextScope: true));

            Days = DaysView.CreateInstance(new CalendarAttributes { StartDate = startDate });
            WeekDays = new WeekDaysView();
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
            set { limitSelectionToRange = value; Change(); }
        }

        public DateTime? MinDate
        {
            get => Days.Attributes.MinDate;
            set => Days.Attributes.MinDate = value;
        }

        public DateTime? MaxDate
        {
            get => Days.Attributes.MaxDate;
            set => Days.Attributes.MaxDate = value;
        }

        public DateTime StartDate
        {
            get => startDate;
            set { startDate = value.Date; Change(); }
        }

        public DayOfWeek StartDay
        {
            get => startDay;
            set { startDay = value; Change(); }
        }

        public int MonthsToShow
        {
            get => Days.Attributes.MonthsToShow;
            set => Days.Attributes.MonthsToShow = value;
        }


        public override async Task OnPreRender()
        {
            await base.OnPreRender();
            await Change();
        }

        async Task Change()
        {
            Header.TitleText = StartDate.ToString("MMM yyyy");

            var start = CalendarHelpers.GetCalendarStartDate(StartDate, StartDay);

            Days.Attributes.StartDate = StartDate;

            if (LimitSelectionToRange)
            {
                Header.PreviousEnabled = !(MinDate.HasValue && CalendarHelpers.GetCalendarStartDate(StartDate, StartDay) < MinDate);
                Header.NextEnabled = !(MaxDate.HasValue && start > MaxDate);
            }

            await Add(MainView);
        }

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
                startDate = Years.NextPage().GetAwaiter().GetResult();
            else
                startDate = Days.NextPage();

            Header.TitleText = StartDate.ToString("MMM yyyy");
        }

        void Header_PreviousTapped()
        {
            if (Scope == CalendarScope.Years)
                startDate = Years.PreviousPage().GetAwaiter().GetResult();
            else
                startDate = Days.PreviousPage();
            Header.TitleText = StartDate.ToString("MMM yyyy");
        }

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
            await Change();
        }

        async Task GoToMonths()
        {
            var months = new MonthsView(StartDate);
            months.MonthTapped.Handle(async args =>
            {
                if (!LockScope)
                {
                    startDate = args;
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
            Years = YearsView.CreateInstance(StartDate);
            Years.YearTapped.Handle(async args =>
            {
                if (!LockScope)
                {
                    startDate = args;
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