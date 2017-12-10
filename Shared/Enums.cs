namespace Zebble
{
    partial class Calendar
    {
        public enum CalendarScope { Days, Months, Years }
        enum AnimationType { NextPage, PreviousPage, Change, None }
        enum AttributeChangeType
        {
            NextPage, PreviousPage, StartDate, StartDay, MinMax, SelectedDate, MonthToShow, MultiSelectable, SelectedDates, SpecialDates
        }
    }
}