namespace Zebble
{
    using System;

    public partial class Calendar
    {
        public TextView TitleLabel { get; protected set; }
        public TextView PrevButton { get; protected set; }
        public TextView NextButton { get; protected set; }
        public Stack Header { get; protected set; }

        bool lockScope;
        public bool LockScope
        {
            get => lockScope;
            set
            {
                lockScope = value;
                if (value) TitleLabel.Tapped.Handle(() => NextMonthYearView());
            }
        }

        void NextButtonTapped()
        {
            if (Scope == CalendarScope.Years) NextPrevYears(isNext: true);
            else StartDate = new DateTime(StartDate.Year, StartDate.Month, 1).AddMonths(MonthsToShow);
            ChangeCalendar(CalandarChanges.All);
        }

        void PrevButtonTapped()
        {
            if (Scope == CalendarScope.Years) NextPrevYears(isNext: false);
            else StartDate = new DateTime(StartDate.Year, StartDate.Month, 1).AddMonths(-MonthsToShow);
            ChangeCalendar(CalandarChanges.All);
        }
    }
}