namespace Zebble
{
    using System;

    partial class Calendar
    {
        public class SpecialDate
        {
            DateTime date;

            public SpecialDate(DateTime date) => Date = date;

            public DateTime Date { get => date; set => date = value.Date; }
            public bool Selectable { get; set; }
        }
    }
}