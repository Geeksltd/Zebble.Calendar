using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Zebble
{
    partial class Calendar
    {
        class YearsView : View
        {
            YearsGrid MainGrid;
            DateTime StartDate;
            public AsyncEvent<DateTime> YearTapped;

            public YearsView(DateTime start)
            {
                MainGrid = new YearsGrid(start);
                YearTapped = new AsyncEvent<DateTime>();
                MainGrid.YearTapped.Handle(date => YearTapped.Raise(date));
                StartDate = start;
                Add(MainGrid);
            }
            public async Task NavigateTo(YearsGrid years, AnimationType animationType)
            {
                await new AnimationHelper(this, MainGrid, years, animationType).Run();
                await Remove(MainGrid);
                MainGrid = years;
            }
            public async Task<DateTime> NextPage()
            {
                var years = new YearsGrid(StartDate);
                var result = years.NextPage();
                await NavigateTo(years, AnimationType.NextPage);
                return result;
            }
            public async Task<DateTime> PreviousPage()
            {
                var years = new YearsGrid(StartDate);
                var result = years.PreviousPage();
                await NavigateTo(years, AnimationType.PreviousPage);
                return result;
            }
        }

        class YearsGrid : Grid
        {
            private DateTime startDate;
            List<Calendar.ItemButton> YearButtons;
            public AsyncEvent<DateTime> YearTapped;
            public int YearRows { get; set; } = 4;
            public int YearColumns { get; set; } = 4;

            public YearsGrid(DateTime start)
            {
                Columns = YearColumns;
                YearTapped = new AsyncEvent<DateTime>();
                YearButtons = new List<Calendar.ItemButton>();
                ChangeDate(start);
            }

            public void ChangeDate(DateTime newDate)
            {
                startDate = newDate.Date;
                Update();
            }

            async Task Update()
            {
                YearButtons.Clear();
                for (var row = 0; row < YearRows; row++)
                {
                    for (var column = 0; column < YearColumns; column++)
                    {
                        var temp = (row * YearColumns) + column + 1;
                        var button = new Calendar.ItemButton
                        {
                            Text = $"{startDate.Year + (temp - (YearColumns * YearRows / 2))}",
                            Date = new DateTime(startDate.Year + (temp - (YearColumns * YearRows / 2)), startDate.Month, 1),
                            Id = "Year"
                        };
                        button.Tapped.HandleWith(() => YearTapped.Raise(button.Date.Value));
                        YearButtons.Add(button);
                        await Add(button);
                    }
                }
            }

            public DateTime NextPage()
            {
                NextPrevYears(isNext: true);
                return startDate;
            }
            public DateTime PreviousPage()
            {
                NextPrevYears(isNext: false);
                return startDate;
            }
            void NextPrevYears(bool isNext)
            {
                var next = (YearRows * YearColumns) * (isNext ? 1 : -1);
                foreach (var button in YearButtons)
                {
                    button.Text = string.Format("{0}", int.Parse(button.Text) + next);
                    button.Date = new DateTime(button.Date.Value.Year + next, button.Date.Value.Month, button.Date.Value.Day);
                }
            }
        }
    }
}
