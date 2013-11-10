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
    /// Interaction logic for AddPayment.xaml
    /// </summary>
    public partial class AddPayment : Window
    {
        public AddPaymentViewModel ViewModel { get { return this.DataContext as AddPaymentViewModel; } }
        public AddPayment(Home home, DuePayment selectedDuePayment)
        {
            InitializeComponent();
            this.DataContext = new AddPaymentViewModel(home, selectedDuePayment);
        }

        private void btnOpenFile_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.FileName = string.Empty;
            ofd.DefaultExt = ".jpg";
            ofd.Filter = "image files (*.jpg)|*.jpg";

            bool? result = ofd.ShowDialog();
            if (result == true)
            {
                ViewModel.CheckImageUNCUpload = ofd.FileName;
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(ViewModel.CheckImageUNCUpload))
            {
                FullScreenImage FullScreenImage = new FullScreenImage(ViewModel.CheckImageUNCUpload);
                FullScreenImage.ShowDialog();
            }
        }

        private void chkForfeit_Checked(object sender, RoutedEventArgs e)
        {
            this.txtPayment.IsEnabled = false;
            ViewModel.PaymentAmount = ViewModel.GetTotalDuesForYear();
        }

        private void chkForfeit_Unchecked(object sender, RoutedEventArgs e)
        {
            this.txtPayment.IsEnabled = true;
            ViewModel.PaymentAmount = 0;
        }
    }
}
