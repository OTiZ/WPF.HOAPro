using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using HOAPro.Persistence;
using HOAPro.Services;

namespace HOAPro.Views
{
    public class AddPaymentViewModel : BaseViewModel
    {
        private DuePayment _duePayment;
        public ObservableCollection<Models.DueInvoiceOwed> DueInvoices { get; set; }

        private Models.DueInvoiceOwed _selectedDueInvoice;
        public Models.DueInvoiceOwed SelectedDueInvoice
        {
            get { return this._selectedDueInvoice; }
            set
            {
                if (this._selectedDueInvoice != value)
                {
                    this._selectedDueInvoice = value;
                    OnPropertyChanged("SelectedDueInvoice");
                }
            }
        }

        private string _checkImageUNC;
        public string CheckImageUNC
        {
            get { return this._checkImageUNC; }
            set
            {
                if (this._checkImageUNC != value)
                {
                    this._checkImageUNC = value;
                    OnPropertyChanged("CheckImageUNC");
                }
            }
        }

        private string _checkImageUNCUpload;
        public string CheckImageUNCUpload
        {
            get { return this._checkImageUNCUpload; }
            set
            {
                if (this._checkImageUNCUpload != value)
                {
                    this._checkImageUNCUpload = value;
                    OnPropertyChanged("CheckImageUNCUpload");
                }
            }
        }

        private decimal _paymentAmount;
        public decimal PaymentAmount
        {
            get { return this._paymentAmount; }
            set
            {
                if (this._paymentAmount != value)
                {
                    this._paymentAmount = value;
                    OnPropertyChanged("PaymentAmount");
                }
            }
        }

        private bool? _cashPayment;
        public bool? CashPayment
        {
            get { return this._cashPayment ?? false; }
            set
            {
                if (this._cashPayment != value)
                {
                    this._cashPayment = value;
                    OnPropertyChanged("CashPayment");
                }
            }
        }

        private bool? _forfeited;
        public bool? Forfeited
        {
            get { return this._forfeited ?? false; }
            set
            {
                if (this._forfeited != value)
                {
                    this._forfeited = value;
                    OnPropertyChanged("Forfeited");
                }
            }
        }

        public AddPaymentViewModel(Home home, DuePayment selectedDuePayment)
        {
            DueInvoices = new ObservableCollection<Models.DueInvoiceOwed>();
            using (var context = Persistence.Persistence.CreateContext())
            {
                var list = (from di in context.DueInvoices
                            where di.HomeId == home.HomeId
                            select di).ToList();
                list.ForEach(d => DueInvoices.Add(Models.DueInvoiceOwed.Create(d)));
                OnPropertyChanged("DueInvoices");
            }
            if (selectedDuePayment != null)
            {
                _duePayment = selectedDuePayment;
                this.CheckImageUNC = selectedDuePayment.CheckImageUNC;
                this.PaymentAmount = selectedDuePayment.PaymentAmount ?? 0;
                this.CashPayment = selectedDuePayment.CashPayment;
                this.Forfeited = selectedDuePayment.ForfeitDueToForeclosure;
                var sdi = DueInvoices.Where(d => d.DueInvoicesId == selectedDuePayment.DueInvoicesId).FirstOrDefault();
                if (sdi != null)
                    SelectedDueInvoice = sdi;

            }
        }

        public DuePayment CreatePayment()
        {
            if (SelectedDueInvoice == null) return null;
            DuePayment dp = null;
            if (_duePayment == null)
            {
                dp = new DuePayment();
                dp.DuePaymentsId = Guid.NewGuid();
            }
            else
                dp = _duePayment;
            dp.PaymentAmount = this.PaymentAmount;
            dp.CheckImageUNC = this.CheckImageUNC;
            dp.DueInvoicesId = SelectedDueInvoice.DueInvoicesId;
            dp.CashPayment = this.CashPayment;
            dp.ForfeitDueToForeclosure = this.Forfeited;

            if (!string.IsNullOrWhiteSpace(this.CheckImageUNCUpload))
            {
                IImageFileService ifs = ImageFileService.Create(this.CheckImageUNCUpload);
                var newFile = ifs.CreateLocallyStoredFile(SystemContext.CheckImagePath);
                if (!string.IsNullOrWhiteSpace(newFile))
                {
                    dp.CheckImageUNC = newFile;
                }
            }
            return dp;
        }

        public decimal GetTotalDuesForYear()
        {
            if (DueInvoices != null && DueInvoices.Count > 0)
            {
                if (SelectedDueInvoice == null)
                    SelectedDueInvoice = DueInvoices.Last();
                using (var context = Persistence.Persistence.CreateContext())
                {
                    var y = context.DueYears.Where(d => d.DueYear1 == SelectedDueInvoice.DueYear).FirstOrDefault();
                    if (y != null)
                        return y.DuesAmount;
                    else
                        return 0;
                }
            }
            else
                return 0;
        }
    }
}
