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

namespace HOAPro.Views
{
    /// <summary>
    /// Interaction logic for ManageDuesYears.xaml
    /// </summary>
    public partial class ManageDuesYears : Window
    {
        private bool _isDirty = false;
        public ManageDuesYearsViewModel ViewModel { get { return this.DataContext as ManageDuesYearsViewModel; } }
        public ManageDuesYears()
        {
            InitializeComponent();
            this.DataContext = new ManageDuesYearsViewModel();
            this.Closing += new System.ComponentModel.CancelEventHandler(ManageDuesYears_Closing);
        }

        void ManageDuesYears_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ViewModel.CloseModel();
        }

        private void grdDueYears_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            _isDirty = true;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            if (_isDirty)
            {
                MessageBoxResult result = MessageBox.Show("Do you want to save your changes?", "Save Changes?", MessageBoxButton.YesNoCancel, MessageBoxImage.Question, MessageBoxResult.Cancel, MessageBoxOptions.DefaultDesktopOnly);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        ViewModel.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, ex.Source, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                    }
                }
                else if (result == MessageBoxResult.Cancel)
                {
                    e.Handled = true;
                    return;
                }
            }
            this.Close();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ViewModel.SaveChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.Source, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
            }
            _isDirty = false;
        }
    }
}
