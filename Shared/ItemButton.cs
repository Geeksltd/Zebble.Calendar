namespace Zebble
{
    using System;

    partial class Calendar
    {
        public class ItemButton : Button
        {
            bool selected, outOfMonth;

            public DateTime? Date { get; set; }

            public bool Selected
            {
                get => selected;
                set { selected = value; SetPseudoCssState("active", value); }
            }

            public bool OutOfMonth
            {
                get => outOfMonth;
                set { outOfMonth = value; SetPseudoCssState("out-of-month", value); }
            }

            internal void SetDisabled() => Enabled = Selected = false;

            internal void Select() => Enabled = Selected = true;
        }
    }
}