using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Zebble
{
    partial class Calendar
    {
        class DaysView : View
        {
            DaysGrid MainGrid;

            public CalendarAttributes Attributes { set; get; }

            private DaysView(CalendarAttributes attributes)
            {
                Attributes = attributes;
                Attributes.AttributeChanged.Handle(async type => await AttributesChanged(type));
                MainGrid = GetNewGrid();
                Add(MainGrid);
            }

            public static DaysView CreateInstance(CalendarAttributes attributes) => new DaysView(attributes);

            async Task NavigateTo(DaysGrid days, AnimationType animationType)
            {
                await new AnimationHelper(this, MainGrid, days, animationType).Run();
                await Remove(MainGrid);
                MainGrid = days;
            }

            public DateTime NextPage()
            {
                var result = new DateTime(Attributes.StartDate.Year, Attributes.StartDate.Month, 1).AddMonths(Attributes.MonthsToShow);
                Attributes.StartDate = result;
                return result;
            }

            public DateTime PreviousPage()
            {
                var result = new DateTime(Attributes.StartDate.Year, Attributes.StartDate.Month, 1).AddMonths(-Attributes.MonthsToShow);
                Attributes.StartDate = result;
                return result;
            }

            async Task AttributesChanged(AttributeChangeType change)
            {
                switch (change)
                {
                    case AttributeChangeType.NextPage:
                        await NavigateTo(GetNewGrid(), AnimationType.NextPage);
                        break;
                    case AttributeChangeType.PreviousPage:
                        await NavigateTo(GetNewGrid(), AnimationType.PreviousPage);
                        break;
                    default:
                        await NavigateTo(GetNewGrid(), AnimationType.Change);
                        break;
                }
            }

            DaysGrid GetNewGrid() => DaysGrid.CreateInstance(Attributes);
        }

        class DaysGrid : Grid
        {
            List<ItemButton> Buttons;

            CalendarAttributes Attributes;

            private DaysGrid(CalendarAttributes attributes)
            {
                Attributes = attributes.Clone();
                Columns = CalendarHelpers.WEEK_DAYS;
                Buttons = new List<ItemButton>(CalendarHelpers.DAYS_IN_MONTH_VIEW);
                CreateButtons();
                Update();
            }

            public static DaysGrid CreateInstance(CalendarAttributes attributes) => new DaysGrid(attributes);

            DateTime? SelectedDate
            {
                get => Attributes.SelectedDate;
                set
                {
                    Attributes.SelectedDate = value?.Date;
                    if (ChangeSelectedDate(Attributes.SelectedDate)) Attributes.SelectedDate = null;
                }
            }

            async Task CreateButtons()
            {
                for (var row = 0; row < CalendarHelpers.MAX_WEEK_IN_MONTH; row++)
                {
                    for (var column = 0; column < CalendarHelpers.WEEK_DAYS; column++)
                    {
                        var button = new ItemButton()
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

            void Update()
            {
                var start = CalendarHelpers.GetCalendarStartDate(Attributes.StartDate, Attributes.StartDay);
                var beginOfMonth = false;
                var endOfMonth = false;

                for (var i = 0; i < Buttons.Count; i++)
                {
                    endOfMonth |= beginOfMonth && start.Day == 1;
                    beginOfMonth |= start.Day == 1;

                    Buttons[i].Text = $"{start.Day}";
                    Buttons[i].Date = start;
                    Buttons[i].OutOfMonth = !(beginOfMonth && !endOfMonth);
                    Buttons[i].Enabled = Attributes.MonthsToShow == 1 || !Buttons[i].OutOfMonth;

                    var specialDate = Attributes.SpecialDates?.FirstOrDefault(s => s.Date == start);

                    Unselect(Buttons[i]);

                    if (start < Attributes.MinDate || start > Attributes.MaxDate) Buttons[i].SetDisabled();
                    else if (Buttons[i].Enabled && Attributes.SelectedDates.Contains(start))
                        Buttons[i].Select();
                    else if (specialDate != null) Buttons[i].Enabled = specialDate.Selectable;

                    start = start.AddDays(1);
                    if (i != 0)
                    {
                        if ((i + 1) % CalendarHelpers.DAYS_IN_MONTH_VIEW == 0)
                        {
                            beginOfMonth = false;
                            endOfMonth = false;
                            start = CalendarHelpers.GetCalendarStartDate(start, Attributes.StartDay);
                        }
                    }
                }
            }

            Task ItemButtonTapped(TouchEventArgs args)
            {
                var item = args.View as ItemButton;

                var selectedDate = item?.Date;
                if (SelectedDate.HasValue && selectedDate.HasValue && SelectedDate == selectedDate)
                {
                    ChangeSelectedDate(selectedDate);
                    SelectedDate = null;
                }
                else SelectedDate = selectedDate;

                return Task.CompletedTask;
            }

            void Unselect(ItemButton button)
            {
                button.Selected = false;
                button.Enabled = Attributes.MonthsToShow == 1 || !button.OutOfMonth;
            }

            bool ChangeSelectedDate(DateTime? date)
            {
                if (date == null) return false;

                if (!Attributes.MultiSelectable)
                {
                    Buttons.FindAll(b => b.Selected).ForEach(ResetButton);
                    Attributes.SelectedDates.Clear();
                }

                Attributes.SelectedDates.Add(SelectedDate.Value);

                var button = Buttons.FirstOrDefault(b => b.Date == date && b.Enabled);
                if (button == null) return false;

                var deselect = button.Selected;
                if (button.Selected) ResetButton(button);
                else
                {
                    Attributes.SelectedDates.Add(SelectedDate.Value);
                    button.Select();
                }

                return deselect;
            }

            void ResetButton(ItemButton button)
            {
                if (button.Date.HasValue) Attributes.SelectedDates.Remove(button.Date.Value);
                var spD = Attributes.SpecialDates?.FirstOrDefault(s => s.Date == button.Date);
                Unselect(button);
                if (spD != null) button.Enabled = spD.Selectable;
            }
        }
    }
}