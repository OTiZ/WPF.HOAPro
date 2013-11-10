using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using HOAPro.Persistence;

namespace HOAPro.Views
{
    public class ManageHomeInvoicesAndPaymentsViewModel : BaseViewModel
    {
        public ObservableCollection<Models.HomeInvoice> HomeInvoices { get; set; }

        private Home _selectedHome;
        public Home SelectedHome
        {
            get { return this._selectedHome; }
            set
            {
                if (value != _selectedHome)
                {
                    this._selectedHome = value;
                    OnPropertyChanged("SelectedHome");
                }
            }
        }

        private Models.HomeInvoice _selectedHomeInvoice;
        public Models.HomeInvoice SelectedHomeInvoice
        {
            get { return this._selectedHomeInvoice; }
            set
            {
                if (value != this._selectedHomeInvoice)
                {
                    this._selectedHomeInvoice = value;
                    OnPropertyChanged("SelectedHomeInvoice");
                }
            }
        }

        public ManageHomeInvoicesAndPaymentsViewModel(Home home)
        {
            SelectedHome = home;
            HomeInvoices = new ObservableCollection<Models.HomeInvoice>();
            HydrateHomeInvoices();
        }

        public void HydrateHomeInvoices()
        {
            using (var context = Persistence.Persistence.CreateContext())
            {
                HomeInvoices.Clear();
                context.DuePayments.Where(d => d.DueInvoice.HomeId == SelectedHome.HomeId).ToList().ForEach(d => HomeInvoices.Add(Models.HomeInvoice.Create(d)));
                OnPropertyChanged("HomeInvoices");
            }
        }

        public void CreateDuePayment(DuePayment duePayment)
        {
            using (var context = Persistence.Persistence.CreateContext())
            {
                context.DuePayments.AddObject(duePayment);
                context.SaveChanges();
            }
        }

        public void UpdateDuePayment(DuePayment duePayment)
        {
            using (var context = Persistence.Persistence.CreateContext())
            {
                var dp = context.DuePayments.Where(d => d.DuePaymentsId == duePayment.DuePaymentsId).FirstOrDefault();
                if (dp != null)
                {
                    dp.CashPayment = duePayment.CashPayment;
                    dp.CheckImageUNC = duePayment.CheckImageUNC;
                    dp.DueInvoicesId = duePayment.DueInvoicesId;
                    dp.ForfeitDueToForeclosure = duePayment.ForfeitDueToForeclosure;
                    dp.PaymentAmount = duePayment.PaymentAmount;
                    context.SaveChanges();
                }
            }
        }

        public void DeleteSelectedHomeInvoice()
        {
            if (SelectedHomeInvoice != null)
            {
                using (var context = Persistence.Persistence.CreateContext())
                {
                    DuePayment dp = context.DuePayments.Where(d => d.DuePaymentsId == SelectedHomeInvoice.DuePaymentId).FirstOrDefault();
                    if (dp != null)
                    {
                        context.DuePayments.DeleteObject(dp);
                        context.SaveChanges();
                    }
                }
                HydrateHomeInvoices();
            }
        }
    }
}
