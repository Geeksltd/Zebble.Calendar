using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace Zebble
{
    public partial class Calendar
    {
        public int YearRows { get; set; } = 4;
        public int YearColumns { get; set; } = 4;

        // TODO: Remove these? Everything should be CSS.
        double _Width, _Height;

        Stack TemporatyView;

        List<ItemButton> YearButtons;

        public CalendarScope Scope { get; protected set; }

        public async Task PrevMonthYearView()
        {
            if (TemporatyView == null)
            {
                YearButtons = new List<ItemButton>();
                await CloneAndClearContentView();
                _Width = ContentView.Width.CurrentValue / MonthsToShow;
                _Height = ContentView.Height.CurrentValue / MonthsToShow;
            }
            switch (Scope)
            {
                case CalendarScope.Days: await ShowYears(); break;
                case CalendarScope.Months: await ShowDays(); break;
                case CalendarScope.Years: await ShowMonths(); break;
                default: await ShowDays(); break;
            }
        }

        public async Task NextMonthYearView()
        {
            if (TemporatyView == null)
            {
                await CloneAndClearContentView();
                YearButtons = new List<ItemButton>();
                _Width = ContentView.Width.CurrentValue / MonthsToShow;
                _Height = ContentView.Height.CurrentValue / MonthsToShow;
            }
            switch (Scope)
            {
                case CalendarScope.Days: await ShowMonths(); break;
                case CalendarScope.Months: await ShowYears(); break;
                case CalendarScope.Years: await ShowDays(); break;
                default: await ShowDays(); break;
            }
        }

        async Task CloneAndClearContentView()
        {
            if (TemporatyView == null)
                TemporatyView = new Stack().Absolute().Hide();
            var children = ContentView.AllChildren;

            if (children.Any(x => x.Native != null))
                await Root.Add(TemporatyView, awaitNative: true);

            foreach (var child in children)
                await child.MoveTo(TemporatyView);
        }

        async Task ReAddToContentView()
        {
            await ContentView.ClearChildren();
            var children = TemporatyView.AllChildren;
            foreach (var child in children)
                await child.MoveTo(ContentView);
            TemporatyView = null;
        }

        public async Task ShowDays()
        {
            Scope = CalendarScope.Days;
            TitleLeftArrow.Visible = TitleRightArrow.Visible = true;
            await ContentView.ClearChildren();
            await ReAddToContentView();
        }

        public async Task ShowMonths()
        {
            var details = new Grid { Columns = 3 };
            for (var row = 0; row < 4; row++)
            {
                for (var column = 0; column < 3; column++)
                {
                    var button = new ItemButton
                    {
                        Text = DateTimeFormatInfo.CurrentInfo.MonthNames[(row * 3) + column],
                        Date = new DateTime(StartDate.Year, (row * 3) + column + 1, 1)
                    };

                    button.Tapped.Handle(async args =>
                    {
                        if (EnableTitleMonthYearView)
                        {
                            StartDate = button.Date.Value;
                            await PrevMonthYearView();
                        }
                    });
                    await details.Add(button);
                }
            }

            details.Width(_Width);
            details.Height(_Height);

            await ContentView.ClearChildren();
            await ContentView.Add(details);

            Scope = CalendarScope.Months;
            TitleLeftArrow.Visible = TitleRightArrow.Visible = false;
        }

        public async Task ShowYears()
        {
            YearButtons.Clear();

            var details = new Grid { Columns = 4 };

            for (var row = 0; row < YearRows; row++)
            {
                for (var column = 0; column < YearColumns; column++)
                {
                    var temp = (row * YearColumns) + column + 1;
                    var button = new ItemButton
                    {
                        Text = $"{StartDate.Year + (temp - (YearColumns * YearRows / 2))}",
                        Date = new DateTime(StartDate.Year + (temp - (YearColumns * YearRows / 2)), StartDate.Month, 1)
                    };

                    button.Width(ContentView.Width.CurrentValue / YearRows);
                    button.Height(ContentView.Height.CurrentValue / YearColumns);
                    button.Tapped.Handle(async args =>
                        {
                            if (EnableTitleMonthYearView)
                            {
                                StartDate = button.Date.Value;
                                await PrevMonthYearView();
                            }
                        }
                    );
                    YearButtons.Add(button);
                    await details.Add(button);
                }
            }
            details.Width(_Width);
            details.Height(_Height);
            await ContentView.ClearChildren();
            await ContentView.Add(details);
            Scope = CalendarScope.Years;
            TitleLeftArrow.Visible = true;
            TitleRightArrow.Visible = true;
        }

        protected void NextPrevYears(bool isNext)
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