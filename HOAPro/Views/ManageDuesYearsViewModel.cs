using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects;
using System.Collections.ObjectModel;
using HOAPro.Persistence;

namespace HOAPro.Views
{
    public class ManageDuesYearsViewModel : BaseViewModel
    {
        public ObservableCollection<Persistence.DueYear> DueYears { get; set; }

        private HOAProDBContainer context = null;
        public ManageDuesYearsViewModel()
        {
            context = Persistence.Persistence.CreateContext();
            this.DueYears = context.DueYears.ToObservableCollection();
        }

        public void CloseModel()
        {
            context.Dispose();
        }

        public void SaveChanges()
        {
            foreach (var item in this.DueYears)
            {
                if (item.DueYearsId == Guid.Empty)
                {
                    item.DueYearsId = Guid.NewGuid();
                    context.DueYears.AddObject(item);
                }
            }
            context.SaveChanges();
        }        
    }
}
