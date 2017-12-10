using System;
using System.Globalization;
using System.Threading.Tasks;

namespace Zebble
{
    partial class Calendar
    {
        class MonthsView : Grid
        {
            DateTime startDate;

            public AsyncEvent<DateTime> MonthTapped;

            public MonthsView() : this(DateTime.Now) { }

            public MonthsView(DateTime start)
            {
                Columns = 3;
                MonthTapped = new AsyncEvent<DateTime>();
                StartDate = start;
            }

            public DateTime StartDate
            {
                get => startDate;
                set
                {
                    startDate = value.Date;
                    Update();
                }
            }

            async Task Update()
            {
                for (var row = 0; row < 4; row++)
                {
                    for (var column = 0; column < 3; column++)
                    {
                        var button = new Calendar.ItemButton
                        {
                            Text = DateTimeFormatInfo.CurrentInfo.MonthNames[(row * 3) + column],
                            Date = new DateTime(StartDate.Year, (row * 3) + column + 1, 1),
                            Id = "Month"
                        };

                        button.Tapped.HandleWith(() => MonthTapped.Raise(button.Date.Value));
                        await Add(button);
                    }
                }
            }

        }
    }
}
