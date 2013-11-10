using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using HOAPro.Persistence;

namespace HOAPro.Views.Dialogs
{
    public class GetDuesYearViewModel : BaseViewModel
    {
        public ObservableCollection<Persistence.DueYear> DuesYears { get; set; }

        private Persistence.DueYear _selectedDuesYear;
        public Persistence.DueYear SelectedDuesYear
        {
            get { return this._selectedDuesYear; }
            set
            {
                if (this._selectedDuesYear != value)
                {
                    this._selectedDuesYear = value;
                    OnPropertyChanged("SelectedDuesYear");
                }
            }
        }

        public GetDuesYearViewModel()
        {
            DuesYears = new ObservableCollection<Persistence.DueYear>();
            using (var context = Persistence.Persistence.CreateContext())
            {
                context.DueYears.ToList().ForEach(d => DuesYears.Add(d));
                OnPropertyChanged("DuesYears");
            }
        }
    }
}
