namespace Zebble
{
    using System;

    partial class Calendar
    {
        public class SpecialDate
        {
            public DateTime Date { get; set; }
            public bool Selectable { get; set; }

            public SpecialDate(DateTime date) => Date = date;
        }
    }
}