using System;
using System.Collections.Generic;
using System.Linq;

namespace Zebble
{
    public partial class Calendar
    {
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

        public bool MultiSelectable { set; get; }

        public List<DateTime> SelectedDates { set; get; } = new List<DateTime>(1);

        protected bool ChangeSelectedDate(DateTime? date)
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

        protected void ResetButton(ItemButton button)
        {
            if (button.Date.HasValue) SelectedDates.Remove(button.Date.Value);
            var spD = SpecialDates?.FirstOrDefault(s => s.Date == button.Date);
            Unselect(button);
            if (spD != null) button.Enabled = spD.Selectable;
        }
    }
}