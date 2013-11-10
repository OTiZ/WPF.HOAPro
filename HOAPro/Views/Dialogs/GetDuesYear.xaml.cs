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

namespace HOAPro.Views.Dialogs
{
    /// <summary>
    /// Interaction logic for GetDuesYear.xaml
    /// </summary>
    public partial class GetDuesYear : Window
    {
        public GetDuesYearViewModel ViewModel { get { return this.DataContext as GetDuesYearViewModel; } }
        public GetDuesYear()
        {
            InitializeComponent();
            this.DataContext = new GetDuesYearViewModel();
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
