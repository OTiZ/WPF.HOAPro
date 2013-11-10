using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;
using System.IO;
using HOAPro.Services;

namespace HOAPro
{
    public class MainWindowViewModel : BaseViewModel
    {
        #region Properties
        private int _homeCount;
        public string HomesCountText
        {
            get { return string.Format("{0} homes found in the database", _homeCount); }
        }

        private int _duesYearsCount;
        public string DuesCountText
        {
            get { return string.Format("{0} year(s) have been invoiced", _duesYearsCount); }
        }

        private int _homesPastDueCount;
        public string HomesPastDueCountText
        {
            get { return string.Format("{0} homes have past due balances", _homesPastDueCount); }
        }

        private decimal _totalAmountDue;
        public string TotalAmountDueText
        {
            get { return string.Format("{0:C2} remain in unpaid dues", _totalAmountDue); }
        }

        private int _homesInViolationCount;
        public string HomesInViolationCountText
        {
            get { return string.Format("{0} homes are currently in violation", _homesInViolationCount); }
        }

        private decimal _totalFinesDue;
        public string TotalFinesDueText
        {
            get { return string.Format("{0:C2} remain in unpaid fines", _totalFinesDue); }
        }
        #endregion Properties

        public MainWindowViewModel()
        {
            HydrateForm();
            EnsureCheckImagePath();
        }

        public void EnsureCheckImagePath()
        {
            SystemContext.CheckImagePath = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "CheckImages");
            if (!Directory.Exists(SystemContext.CheckImagePath))
            {
                try
                {
                    Directory.CreateDirectory(SystemContext.CheckImagePath);
                }
                catch (Exception ex)
                {
                    throw new ApplicationException(string.Format("Something went wrong attempting to create the path {0}. This path must exist and you must have read/write access to it.", SystemContext.CheckImagePath));
                }
            }
        }

        public void HydrateForm()
        {
            BackgroundWorker bw = new BackgroundWorker();
            bw.WorkerReportsProgress = false;
            bw.WorkerSupportsCancellation = false;
            bw.DoWork += bw_DoWork;
            bw.RunWorkerAsync();
        }

        void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            using (var context = Persistence.Persistence.CreateContext())
            {
                _homeCount = context.Homes.Count();
                OnPropertyChanged("HomesCountText");
                _duesYearsCount = (from dy in context.DueYears
                                   join di in context.DueInvoices on dy.DueYearsId equals di.DueYearsId
                                   select dy.DueYearsId).Distinct().Count();
                OnPropertyChanged("DuesCountText");
                var inv = (from i in context.DueInvoices
                           group i by i.HomeId into d
                           select new TotalInvoicedHomes
                           {
                               HomeId = d.Key,
                               TotalInvoiced = d.Sum(x => x.DueAmount)
                           }).ToList();

                foreach (var item in inv)
                {
                    item.TotalPaid = (from di in context.DueInvoices
                                      join dp in context.DuePayments on di.DueInvoicesId equals dp.DueInvoicesId into di_dp
                                      from dues in di_dp.DefaultIfEmpty()
                                      where di.HomeId == item.HomeId
                                      select dues).Sum(d => d.PaymentAmount ?? 0);
                }


                _homesPastDueCount = (from i in inv
                                      where i.TotalInvoiced > i.TotalPaid
                                      select i.HomeId).Count();
                OnPropertyChanged("HomesPastDueCountText");
                _totalAmountDue = (from i in inv
                                   where i.TotalInvoiced > i.TotalPaid
                                   select i.TotalInvoiced - i.TotalPaid).Sum();
                OnPropertyChanged("TotalAmountDueText");
            }            
        }
    }

    public class TotalInvoicedHomes
    {
        public int HomeId { get; set; }
        public decimal TotalInvoiced { get; set; }
        public decimal TotalPaid { get; set; }
    }
    public class TotalPaidHomes
    {
        public int HomeId { get; set; }
        public decimal TotalPaid { get; set; }
    }
}
