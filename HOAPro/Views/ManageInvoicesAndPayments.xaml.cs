using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace HOAPro.Views
{
    /// <summary>
    /// Interaction logic for ManageInvoicesAndPayments.xaml
    /// </summary>
    public partial class ManageInvoicesAndPayments : Window
    {
        public ManageInvoicesAndPaymentsViewModel ViewModel { get { return this.DataContext as ManageInvoicesAndPaymentsViewModel; } }
        public ManageInvoicesAndPayments()
        {
            InitializeComponent();
            this.DataContext = new ManageInvoicesAndPaymentsViewModel();
        }

        private void btnManagePayments_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.ShowManageHomeInvoicesandPayments();
        }

        private void btnRunInvoices_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.RunInvoices();
        }

        private void btnExportPastDue_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.FileName = "PastDueHomes";
            dlg.DefaultExt = ".xlsx";
            dlg.Filter = "Microsoft Excel Files (.xlsx)|*.xlsx";
            dlg.CheckFileExists = false;
            dlg.OverwritePrompt = false;
            if (dlg.ShowDialog().GetValueOrDefault(false))
            {
                ViewModel.ExportToExcel(dlg.FileName);
            }
        }
    }
}
