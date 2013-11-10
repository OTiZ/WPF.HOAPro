using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using HOAPro.Persistence;

namespace HOAPro.Models
{
    public class InvoiceHomePayment : BaseViewModel
    {
        public Home Home { get; set; }
        
        private decimal _TotalInvoiced;
        public decimal TotalInvoiced 
        {
            get { return this._TotalInvoiced; }
            set
            {
                if (value != this._TotalInvoiced)
                {
                    this._TotalInvoiced = value;
                    OnPropertyChanged("TotalInvoiced");
                }
            }
        }

        private decimal _TotalPayments;
        public decimal TotalPayments 
        {
            get { return this._TotalPayments; }
            set
            {
                if (this._TotalPayments != value)
                {
                    this._TotalPayments = value;
                    OnPropertyChanged("TotalPayments");
                }
            }
        }
        public string Address { get; private set; }


        public void Recalculate(Home home)
        {
            Home localHome = home ?? this.Home;
            
            if (localHome != null)
            {
                this.TotalInvoiced = localHome.DueInvoices.Sum(d => d.DueAmount);
                this.TotalPayments = localHome.DueInvoices.Sum(d => d.DuePayments.Sum(x => x.PaymentAmount.GetValueOrDefault(0)));
            }
        }
        public void Recalculate()
        {
            this.Recalculate(null);
        }

        public static InvoiceHomePayment Create(Home home)
        {
            InvoiceHomePayment ihp = new InvoiceHomePayment();
            ihp.Home = home;
            ihp.Address = home.PhysicalAddress;
            ihp.Recalculate(home);
            return ihp;
        }
    }
}
