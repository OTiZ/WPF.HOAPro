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
    /// Interaction logic for ProgressWindow.xaml
    /// </summary>
    public partial class ProgressWindow : Window
    {
        public delegate void UpdateRecordsProcessedDelegate(object o, int percentageComplete);
        public event UpdateRecordsProcessedDelegate UpdateRecordsProcessed;

        public ProgressWindowViewModel ViewModel { get { return this.DataContext as ProgressWindowViewModel; } }
        public ProgressWindow(string baseProgressText, double totalRecords)
        {
            InitializeComponent();
            this.DataContext = new ProgressWindowViewModel(baseProgressText, totalRecords);
        }

        public void SetPercentComplete(int percentComplete, int recordsProcessed)
        {
            ViewModel.PercentComplete = percentComplete;
            ViewModel.RecordsProcessed = recordsProcessed;
        }
    }
}
