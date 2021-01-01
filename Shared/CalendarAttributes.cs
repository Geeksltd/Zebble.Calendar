using System;
using System.Collections.Generic;
using Olive;

namespace Zebble
{
    partial class Calendar
    {
        class CalendarAttributes
        {
            public AsyncEvent<AttributeChangeType> AttributeChanged = new AsyncEvent<AttributeChangeType>();

            DateTime startDate = DateTime.Today;
            public DateTime StartDate
            {
                set
                {
                    if (value == startDate) return;

                    if (Math.Abs((StartDate - value).TotalDays) > 0 && Math.Abs((StartDate - value).TotalDays) <= CalendarHelpers.DAYS_IN_MONTH_VIEW)
                    {
                        startDate = value;
                        AttributeChanged.Raise(StartDate < value
                            ? AttributeChangeType.NextPage
                            : AttributeChangeType.PreviousPage);
                    }
                    else
                    {
                        startDate = value;
                        AttributeChanged.Raise(AttributeChangeType.StartDate);
                    }
                }
                get => startDate;
            }

            DateTime? maxDate;
            public DateTime? MaxDate
            {
                set
                {
                    if (value == maxDate) return;
                    maxDate = value;
                    AttributeChanged.Raise(AttributeChangeType.MinMax);
                }
                get => maxDate;
            }

            DateTime? minDate;
            public DateTime? MinDate
            {
                set
                {
                    if (value == minDate) return;
                    minDate = value;
                    AttributeChanged.Raise(AttributeChangeType.MinMax);
                }
                get => minDate;
            }

            DayOfWeek startDay = DayOfWeek.Sunday;
            public DayOfWeek StartDay
            {
                set
                {
                    if (value == startDay) return;
                    startDay = value;
                    AttributeChanged.Raise(AttributeChangeType.StartDay);
                }
                get => startDay;
            }

            DateTime? selectedDate;
            public DateTime? SelectedDate
            {
                set
                {
                    if (value == selectedDate) return;
                    selectedDate = value;
                    AttributeChanged.Raise(AttributeChangeType.SelectedDate);
                }
                get => selectedDate;
            }

            int monthsToShow = 1;
            public int MonthsToShow
            {
                set
                {
                    if (value == monthsToShow) return;
                    monthsToShow = value;
                    AttributeChanged.Raise(AttributeChangeType.MonthToShow);
                }
                get => monthsToShow;
            }

            bool multiSelectable;
            public bool MultiSelectable
            {
                set
                {
                    if (value == multiSelectable) return;
                    multiSelectable = value;
                    AttributeChanged.Raise(AttributeChangeType.MultiSelectable);
                }
                get => multiSelectable;
            }

            List<DateTime> selectedDates = new List<DateTime>(1);
            public List<DateTime> SelectedDates
            {
                set
                {
                    selectedDates = value;
                    AttributeChanged.Raise(AttributeChangeType.SelectedDates);
                }
                get => selectedDates;
            }

            List<SpecialDate> specialDates = new List<SpecialDate>();
            public List<SpecialDate> SpecialDates
            {
                set
                {
                    specialDates = value;
                    AttributeChanged.Raise(AttributeChangeType.SpecialDates);
                }
                get => specialDates;
            }

            public CalendarAttributes Clone()
            {
                return new CalendarAttributes
                {
                    AttributeChanged = new AsyncEvent<AttributeChangeType>(),
                    StartDate = StartDate,
                    MaxDate = MaxDate,
                    MinDate = MinDate,
                    MonthsToShow = MonthsToShow,
                    MultiSelectable = MultiSelectable,
                    SelectedDate = SelectedDate,
                    StartDay = StartDay,
                    SelectedDates = SelectedDates.Clone(),
                    SpecialDates = SpecialDates.Clone()
                };
            }
        }
    }
}