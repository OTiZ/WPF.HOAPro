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
using HOAPro.Views.Dialogs;
using HOAPro.Persistence;

namespace HOAPro.Views
{
    /// <summary>
    /// Interaction logic for ManageHomeInvoicesandPayments.xaml
    /// </summary>
    public partial class ManageHomeInvoicesandPayments : Window
    {
        public ManageHomeInvoicesAndPaymentsViewModel ViewModel { get { return this.DataContext as ManageHomeInvoicesAndPaymentsViewModel; } }
        public ManageHomeInvoicesandPayments(Home home)
        {
            InitializeComponent();
            this.DataContext = new ManageHomeInvoicesAndPaymentsViewModel(home);
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void btnDeleteRow_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.DeleteSelectedHomeInvoice();
        }

        private void btnAddPayment_Click(object sender, RoutedEventArgs e)
        {
            AddPayment AddPayment = new AddPayment(ViewModel.SelectedHome, null);
            bool? result = AddPayment.ShowDialog();
            if(result == true)
            {
                DuePayment dp = AddPayment.ViewModel.CreatePayment();
                if (dp != null)
                {
                    ViewModel.CreateDuePayment(dp);
                }
                ViewModel.HydrateHomeInvoices();
            }
        }

        private void btnEditPayment_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.SelectedHomeInvoice == null) return;
            using(var context = Persistence.Persistence.CreateContext())
            {
                var DuePayment = context.DuePayments.Where(d => d.DuePaymentsId == ViewModel.SelectedHomeInvoice.DuePaymentId).FirstOrDefault();
                if(DuePayment != null)
                {
                    AddPayment AddPayment = new AddPayment(ViewModel.SelectedHome, DuePayment);
                    bool? result = AddPayment.ShowDialog();
                    if(result == true)
                    {
                        DuePayment dp = AddPayment.ViewModel.CreatePayment();
                        if (dp != null)
                        {
                            ViewModel.UpdateDuePayment(dp);
                        }
                        ViewModel.HydrateHomeInvoices();
                    }
                }
            }
        }

        private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (ViewModel.SelectedHomeInvoice != null && !string.IsNullOrWhiteSpace(ViewModel.SelectedHomeInvoice.CheckImageUNC))
            {
                FullScreenImage dialog = new FullScreenImage(ViewModel.SelectedHomeInvoice.CheckImageUNC);
                dialog.ShowDialog();
            }
        }
    }
}
