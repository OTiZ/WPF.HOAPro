using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Data.Objects;
using HOAPro.Persistence;

namespace HOAPro.Views
{
    public class ManageHomesViewModel : BaseViewModel
    {
        public ObjectSet<Home> Homes { get; set; }

        private HOAProDBContainer context = null;
        public ManageHomesViewModel()
        {
            context = Persistence.Persistence.CreateContext();
            Homes = context.Homes;
        }

        public void CloseModel()
        {
            context.Dispose();
        }

        public void SaveChanges()
        {
            context.SaveChanges();
        }
    }
}
