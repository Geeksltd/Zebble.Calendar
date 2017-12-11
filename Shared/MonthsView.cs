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

            private MonthsView(DateTime start)
            {
                Columns = 3;
                MonthTapped = new AsyncEvent<DateTime>();
                ChangeStartDate(start);
            }

            public static MonthsView CreateInstance(DateTime start) => new MonthsView(start);

            public async Task ChangeStartDate(DateTime start)
            {
                startDate = start;
                await Update();
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
                            Date = new DateTime(startDate.Year, (row * 3) + column + 1, 1),
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