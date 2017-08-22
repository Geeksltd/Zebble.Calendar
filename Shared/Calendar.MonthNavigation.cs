namespace Zebble
{
    using System;

    public partial class Calendar
    {
        public TextView TitleLabel { get; protected set; }
        public ItemButton TitleLeftArrow { get; protected set; }
        public ItemButton TitleRightArrow { get; protected set; }
        public Stack MonthNavigationLayout { get; protected set; }

        bool enableTitleMonthYearView = true;
        public bool EnableTitleMonthYearView
        {
            get => enableTitleMonthYearView;
            set
            {
                enableTitleMonthYearView = value;
                if (value) TitleLabel.Tapped.Handle(() => NextMonthYearView());
            }
        }

        void RightArrowTapped()
        {
            if (Scope == CalendarScope.Years) NextPrevYears(isNext: true);
            else StartDate = new DateTime(StartDate.Year, StartDate.Month, 1).AddMonths(MonthsToShow);
        }

        void LeftArrowTapped()
        {
            if (Scope == CalendarScope.Years) NextPrevYears(isNext: false);
            else StartDate = new DateTime(StartDate.Year, StartDate.Month, 1).AddMonths(-MonthsToShow);
        }
    }
}