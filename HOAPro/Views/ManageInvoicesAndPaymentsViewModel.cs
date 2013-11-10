using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using HOAPro.Views.Dialogs;
using Microsoft.Office.Interop.Excel;
using HOAPro.Persistence;

namespace HOAPro.Views
{
    public class ManageInvoicesAndPaymentsViewModel : BaseViewModel
    {
        public static int x = 0;
        public ObservableCollection<Models.InvoiceHomePayment> InvoiceHomePayments { get; set; }

        private Models.InvoiceHomePayment _selectedInvoiceHomePayment;
        public Models.InvoiceHomePayment SelectedInvoiceHomePayment
        {
            get { return this._selectedInvoiceHomePayment; }
            set
            {
                if (value != _selectedInvoiceHomePayment)
                {
                    this._selectedInvoiceHomePayment = value;
                    OnPropertyChanged("SelectedInvoiceHomePayment");
                }
            }
        }

        public ManageInvoicesAndPaymentsViewModel()
        {
            InvoiceHomePayments = new ObservableCollection<Models.InvoiceHomePayment>();
            HydrateInvoiceHomePayments();
        }

        private void HydrateInvoiceHomePayments()
        {
            InvoiceHomePayments.Clear();
            using (var context = Persistence.Persistence.CreateContext())
            {
                context.Homes.ToList().ForEach(d => InvoiceHomePayments.Add(Models.InvoiceHomePayment.Create(d)));
                OnPropertyChanged("InvoiceHomePayments");
            }
        }

        public void ShowManageHomeInvoicesandPayments()
        {
            if (SelectedInvoiceHomePayment != null && SelectedInvoiceHomePayment.Home != null)
            {
                ManageHomeInvoicesandPayments managehomeinvoices = new ManageHomeInvoicesandPayments(SelectedInvoiceHomePayment.Home);
                managehomeinvoices.Closed += (s, e) =>
                {
                    using (var context = Persistence.Persistence.CreateContext())
                    {
                        var home = context.Homes.Where(d => d.HomeId == SelectedInvoiceHomePayment.Home.HomeId).Single();
                        SelectedInvoiceHomePayment.Recalculate(home);
                    }
                };
                managehomeinvoices.ShowDialog();
            }
        }

        public void RunInvoices()
        {
            GetDuesYear GetDuesYear = new GetDuesYear();
            GetDuesYear.ShowDialog();
            if (GetDuesYear.DialogResult.GetValueOrDefault(false))
            {
                DueYear dueYear = GetDuesYear.ViewModel.SelectedDuesYear;
                if (dueYear != null)
                {
                    double homesCount = 0;
                    using (var context = Persistence.Persistence.CreateContext())
                    {
                        homesCount = context.Homes.Count();
                    }
                    ProgressWindow progressWindow = new ProgressWindow("Processing {0} of {1} records...", homesCount);
                    progressWindow.UpdateRecordsProcessed += new ProgressWindow.UpdateRecordsProcessedDelegate(progressWindow_UpdateRecordsProcessed);
                    BackgroundWorker bgw = new BackgroundWorker();
                    bgw.WorkerReportsProgress = true;
                    bgw.DoWork += delegate(object s, DoWorkEventArgs args)
                    {
                        using (var context = Persistence.Persistence.CreateContext())
                        {
                            var homes = context.Homes.ToList();
                            x = 0;
                            foreach (var home in homes)
                            {
                                x++;
                                bgw.ReportProgress(Convert.ToInt32(((decimal)x / (decimal)homesCount) * 100));
                                var exists = (from i in context.DueInvoices where i.DueYearsId == dueYear.DueYearsId && i.HomeId == home.HomeId select i).FirstOrDefault();
                                if (exists == null)
                                {
                                    Persistence.DueInvoice invoice = new Persistence.DueInvoice();
                                    invoice.HomeId = home.HomeId;
                                    invoice.DueYearsId = dueYear.DueYearsId;
                                    invoice.DueAmount = dueYear.DuesAmount;
                                    invoice.DueInvoicesId = Guid.NewGuid();
                                    context.DueInvoices.AddObject(invoice);
                                    context.SaveChanges();
                                }
                            }
                        }
                    };

                    bgw.RunWorkerCompleted += delegate(object s, RunWorkerCompletedEventArgs args)
                    {
                        HydrateInvoiceHomePayments();
                        string exportFile = string.Format(System.IO.Directory.GetCurrentDirectory() + @"\{0}Invoices.xlsx", dueYear.DueYear1);
                        ExportInvoicesToExcel(dueYear, exportFile);
                        progressWindow.Close();
                        System.Windows.MessageBox.Show(string.Format("Invoice data has been exported to the following Excel file for mail merging purposes: {0}", exportFile));
                    };

                    bgw.ProgressChanged += delegate(object s, ProgressChangedEventArgs args) 
                    { 
                        int percentage = args.ProgressPercentage;
                        progressWindow_UpdateRecordsProcessed(progressWindow, percentage);
                    };

                    bgw.RunWorkerAsync();
                    progressWindow.ShowDialog();
                }
            }
        }

        public void ExportInvoicesToExcel(DueYear dueYear, string exportFile)
        {
            Microsoft.Office.Interop.Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();
            if (xlApp == null)
            {
                System.Windows.MessageBox.Show("You must have Microsoft Excel 2007 or greater installed and have administrator or power user permissions in order to export to excel.", "Export Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                return;
            }

            Workbooks wbs = xlApp.Workbooks;
            Workbook wb = wbs.Add(XlWBATemplate.xlWBATWorksheet);
            Worksheet ws = (Worksheet)wb.Worksheets[1];

            if (ws != null)
            {
                ws.Cells[1, "A"] = "Year";
                ws.Cells[1, "B"] = "Address";
                ws.Cells[1, "C"] = "BillingAddress";
                ws.Cells[1, "D"] = "TotalDue";
            }
            using (var context = Persistence.Persistence.CreateContext())
            {
                var row = 1;
                foreach (var home in context.Homes)
                {
                    row++;
                    ws.Cells[row, "A"] = dueYear.DueYear1;
                    ws.Cells[row, "B"] = home.PhysicalAddress;
                    ws.Cells[row, "C"] = home.MailingAddress;
                    ws.Cells[row, "D"] = dueYear.DuesAmount.ToString("c");
                }
            }

            ((Microsoft.Office.Interop.Excel.Range)ws.Columns[1]).AutoFit();
            ((Microsoft.Office.Interop.Excel.Range)ws.Columns[2]).AutoFit();
            ((Microsoft.Office.Interop.Excel.Range)ws.Columns[3]).AutoFit();
            ((Microsoft.Office.Interop.Excel.Range)ws.Columns[4]).AutoFit();

            //xlApp.Visible = true;
            wb.SaveAs(exportFile, XlFileFormat.xlWorkbookDefault);
            wb.Close();
            xlApp.Quit();
            NAR(ws);
            NAR(wbs);
            NAR(wb);
            NAR(xlApp);
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        public void ExportToExcel(string exportFile)
        {
            var pastDueHomes = (from ihp in this.InvoiceHomePayments where ihp.TotalInvoiced > ihp.TotalPayments select ihp).ToList();
            if(pastDueHomes == null || pastDueHomes.Count <= 0)
            {
                System.Windows.MessageBox.Show("There were no homes with a past due balance.", "Nothing to Export", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
            }

            Microsoft.Office.Interop.Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();
            if (xlApp == null)
            {
                System.Windows.MessageBox.Show("You must have Microsoft Excel 2007 or greater installed and have administrator or power user permissions in order to export to excel.", "Export Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                return;
            }
            Workbooks wbs = xlApp.Workbooks;
            Workbook wb = wbs.Add(XlWBATemplate.xlWBATWorksheet);
            Worksheet ws = (Worksheet)wb.Worksheets[1];

            if (ws != null)
            {
                ws.Cells[1, "A"] = "Address";
                ws.Cells[1, "B"] = "Year";
                ws.Cells[1, "C"] = "Total";
            }

            using (var context = Persistence.Persistence.CreateContext())
            {
                var row = 1;
                foreach (var home in pastDueHomes)
                {
                    var list = (from di in context.DueInvoices
                                where di.HomeId == home.Home.HomeId
                                select di).ToList();
                    List<Models.DueInvoiceOwed> listx = new List<Models.DueInvoiceOwed>();
                    list.ForEach(d => listx.Add(Models.DueInvoiceOwed.Create(d)));

                    var owed = listx.Where(d => d.DuesOwed > 0);
                    if (owed != null && owed.Count() > 0)
                    {
                        row++;
                        ws.Cells[row, "A"] = home.Home.PhysicalAddress;
                        StringBuilder sb = new StringBuilder();
                        decimal total = 0;
                        foreach (var owe in owed)
                        {
                            sb.AppendLine(owe.DueYear + " Annual Dues          " + owe.DuesOwed.ToString("c"));
                            total += owe.DuesOwed;
                        }
                        ws.Cells[row, "B"] = sb.ToString().Substring(0, sb.ToString().Length - 1);
                        ws.Cells[row, "C"] = total.ToString("c");
                    }
                }
            }
            
            ((Microsoft.Office.Interop.Excel.Range)ws.Columns[1]).AutoFit();
            ((Microsoft.Office.Interop.Excel.Range)ws.Columns[2]).AutoFit();
            ((Microsoft.Office.Interop.Excel.Range)ws.Columns[3]).AutoFit();

            //xlApp.Visible = true;
            wb.SaveAs(exportFile, XlFileFormat.xlWorkbookDefault);
            wb.Close();
            xlApp.Quit();
            NAR(ws);
            NAR(wbs);
            NAR(wb);
            NAR(xlApp);
            GC.Collect();
            GC.WaitForPendingFinalizers();
            System.Windows.MessageBox.Show("The export has completed successfully.", "Export Complete", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
        }

        private void NAR(object o)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(o);
            }
            catch { }
            finally
            {
                o = null;
            }
        }

        void progressWindow_UpdateRecordsProcessed(object o, int percentComplete)
        {
            (o as ProgressWindow).SetPercentComplete(percentComplete, x);
        }
    }
}
