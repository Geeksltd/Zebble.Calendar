using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zebble
{
    class DaysView : Grid
    {
        List<Calendar.ItemButton> Buttons;

        public bool MultiSelectable { set; get; } = false;

        public List<DateTime> SelectedDates { set; get; } = new List<DateTime>(1);

        DateTime startDate = DateTime.Today;
        public DateTime StartDate
        {
            get => startDate;
            set
            {
                startDate = value.Date;
                Update();
            }
        }

        private DateTime? maxDate;

        public DateTime? MaxDate
        {
            get => maxDate;
            set
            {
                maxDate = value;
                Update();
            }
        }

        private DateTime? minDate;

        public DateTime? MinDate
        {
            get => minDate;
            set
            {
                minDate = value;
                Update();
            }
        }

        ////Selected Dates
        DateTime? selectedDate = null;
        public DateTime? SelectedDate
        {
            get => selectedDate;
            set
            {
                selectedDate = value?.Date;
                if (ChangeSelectedDate(selectedDate)) selectedDate = null;
            }
        }

        DayOfWeek startDay = DayOfWeek.Sunday;

        public DayOfWeek StartDay
        {
            get => startDay;
            set
            {
                startDay = value;
                Update();
            }
        }

        List<Calendar.SpecialDate> specialDates = new List<Calendar.SpecialDate>();
        public List<Calendar.SpecialDate> SpecialDates
        {
            get => specialDates;
            set { Update(); specialDates = value; }
        }

        int monthsToShow = 1;
        public int MonthsToShow
        {
            get { return monthsToShow; }
            set { Update(); monthsToShow = value; }
        }

        public DaysView()
        {
            Columns = CalendarHelpers.WEEK_DAYS;
            Buttons = new List<Calendar.ItemButton>(CalendarHelpers.DAYS_IN_MONTH_VIEW);
            CreateButtons();
            Update();
        }

        public int ButtonCount => Buttons.Count;

        public DateTime NextPage()
        {
            StartDate = new DateTime(StartDate.Year, StartDate.Month, 1).AddMonths(MonthsToShow);
            return StartDate;
        }

        public DateTime PreviousPage()
        {
            StartDate = new DateTime(StartDate.Year, StartDate.Month, 1).AddMonths(-MonthsToShow);
            return StartDate;
        }


        async Task CreateButtons()
        {
            for (var row = 0; row < CalendarHelpers.MAX_WEEK_IN_MONTH; row++)
            {
                for (var column = 0; column < CalendarHelpers.WEEK_DAYS; column++)
                {
                    var button = new Calendar.ItemButton()
                    {
                        Id = "Day"
                    };

                    Buttons.Add(button);
                    var lastButton = Buttons.Last();
                    lastButton.Tapped.Handle(ItemButtonTapped);

                    await Add(button);
                }
            }
            await EnsureFullColumns();
        }



        public async Task Update()
        {
            var start = CalendarHelpers.GetCalendarStartDate(StartDate, StartDay);
            var beginOfMonth = false;
            var endOfMonth = false;

            for (var i = 0; i < Buttons.Count; i++)
            {
                endOfMonth |= beginOfMonth && start.Day == 1;
                beginOfMonth |= start.Day == 1;

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
                    if ((i + 1) % CalendarHelpers.DAYS_IN_MONTH_VIEW == 0)
                    {
                        beginOfMonth = false;
                        endOfMonth = false;
                        start = CalendarHelpers.GetCalendarStartDate(start, StartDay);
                    }
                }
            }
        }



        Task ItemButtonTapped(TouchEventArgs args)
        {
            var item = args.View as Calendar.ItemButton;

            var selectedDate = item.Date;
            if (SelectedDate.HasValue && selectedDate.HasValue && SelectedDate == selectedDate)
            {
                ChangeSelectedDate(selectedDate);
                SelectedDate = null;
            }
            else SelectedDate = selectedDate;

            return Task.CompletedTask;
        }

        protected void Unselect(Calendar.ItemButton button)
        {
            button.Selected = false;
            button.Enabled = MonthsToShow == 1 || !button.OutOfMonth;
        }

        private bool ChangeSelectedDate(DateTime? date)
        {
            if (date == null) return false;

            if (!MultiSelectable)
            {
                Buttons.FindAll(b => b.Selected).ForEach(b => ResetButton(b));
                SelectedDates.Clear();
            }

            SelectedDates.Add(SelectedDate.Value);

            var button = Buttons.FirstOrDefault(b => b.Date == date && b.Enabled);
            if (button == null) return false;

            var deselect = button.Selected;
            if (button.Selected) ResetButton(button);
            else
            {
                SelectedDates.Add(SelectedDate.Value);
                button.Select();
            }

            return deselect;
        }

        protected void ResetButton(Calendar.ItemButton button)
        {
            if (button.Date.HasValue) SelectedDates.Remove(button.Date.Value);
            var spD = SpecialDates?.FirstOrDefault(s => s.Date == button.Date);
            Unselect(button);
            if (spD != null) button.Enabled = spD.Selectable;
        }

    }
}
