using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HOAPro.Views.Dialogs
{
    public class ProgressWindowViewModel : BaseViewModel
    {
        private string _baseProgressText;

        private string _progressText;
        public string ProgressText
        {
            get { return this._progressText; }
            set
            {
                if (this._progressText != value)
                {
                    this._progressText = value;
                    OnPropertyChanged("ProgressText");
                }
            }
        }

        private double _recordsProcessed;
        public double RecordsProcessed
        {
            get { return this._recordsProcessed; }
            set
            {
                if (this._recordsProcessed != value)
                {
                    this._recordsProcessed = value;
                    OnPropertyChanged("RecordsProcessed");
                    this.ProgressText = string.Format(_baseProgressText, this.RecordsProcessed, this.TotalRecords);
                    OnPropertyChanged("ProgressText");
                }
            }
        }

        private double _totalRecords;
        public double TotalRecords
        {
            get { return this._totalRecords; }
            set
            {
                if (this._totalRecords != value)
                {
                    this._totalRecords = value;
                    OnPropertyChanged("TotalRecords");
                    this.ProgressText = string.Format(_baseProgressText, this.RecordsProcessed, this.TotalRecords);
                    OnPropertyChanged("ProgressText");
                }
            }
        }

        private double _percentComplete;
        public double PercentComplete
        {
            get { return this._percentComplete; }
            set
            {
                if (this._percentComplete != value)
                {
                    this._percentComplete = value;
                    OnPropertyChanged("PercentComplete");
                }
            }
        }

        public ProgressWindowViewModel(string baseProgressText, double totalRecords)
        {
            _baseProgressText = baseProgressText;
            this.TotalRecords = totalRecords;
            this.RecordsProcessed = 0;
        }
    }
}
