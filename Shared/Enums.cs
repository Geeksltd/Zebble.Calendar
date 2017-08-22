namespace Zebble
{
    partial class Calendar
    {
        public enum CalendarScope { Days, Months, Years }

        [System.Flags]
        public enum CalandarChanges
        {
            MaxMin = 1,
            StartDate = 2,
            StartDay = 4,
            All = MaxMin | StartDate | StartDay
        }
    }
}