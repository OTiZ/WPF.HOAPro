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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HOAPro
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindowViewModel ViewModel { get { return this.DataContext as MainWindowViewModel; } }
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new MainWindowViewModel();
        }

        private void btnManageHomes_Click(object sender, RoutedEventArgs e)
        {
            btnManageHomesContextMenu.PlacementTarget = this;
            btnManageHomesContextMenu.IsOpen = true;
        }

        private void ManageHomesMenuItem_Click(object sender, RoutedEventArgs e)
        {
            HOAPro.Views.ManageHomes ManageHomes = new Views.ManageHomes();
            ManageHomes.ShowDialog();
            ViewModel.HydrateForm();
        }

        private void ManageDuesMenuItem_Click(object sender, RoutedEventArgs e)
        {
            HOAPro.Views.ManageDuesYears ManageDuesYears = new Views.ManageDuesYears();
            ManageDuesYears.ShowDialog();
            ViewModel.HydrateForm();
        }

        private void btnInvoicesAndPayments_Click(object sender, RoutedEventArgs e)
        {
            HOAPro.Views.ManageInvoicesAndPayments ManageInvoicesAndPayments = new Views.ManageInvoicesAndPayments();
            ManageInvoicesAndPayments.ShowDialog();
            ViewModel.HydrateForm();
        }
    }
}
