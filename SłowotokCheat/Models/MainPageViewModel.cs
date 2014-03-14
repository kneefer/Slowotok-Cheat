using SłowotokCheat.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SłowotokCheat.Models
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<RecordModel> FoundWords { get; set; }

        public MainPageViewModel()
        {
            IsBaseLoaded = false;
            FoundWords = new ObservableCollection<RecordModel>();
            InProgress = false;
            ShowingResults = false;
            ArrayOfChars = new char[4][]{
                new char[4] {' ', ' ', ' ', ' '},
                new char[4] {' ', ' ', ' ', ' '},
                new char[4] {' ', ' ', ' ', ' '},
                new char[4] {' ', ' ', ' ', ' '}
            };
        }

        private char[][] _arrayOfChars;
        public char[][] ArrayOfChars
        {
            get { return _arrayOfChars; }
            set
            {
                if (value != _arrayOfChars)
                {
                    _arrayOfChars = value;
                    NotifyPropertyChanged("ArrayOfChars");
                }
            }
        }

        private bool _inProgress;
        public bool InProgress
        {
            get { return _inProgress; }
            set
            {
                if (value != _inProgress)
                {
                    _inProgress = value;
                    NotifyPropertyChanged("InProgress");
                }
            }
        }

        private bool _showingResults;
        public bool ShowingResults
        {
            get { return _showingResults; }
            set
            {
                if (value != _showingResults)
                {
                    _showingResults = value;
                    NotifyPropertyChanged("ShowingResults");
                }
            }
        }
        

        private bool _isBaseLoaded;
        public bool IsBaseLoaded
        {
            get { return _isBaseLoaded; }
            set
            {
                if (value != _isBaseLoaded)
                {
                    _isBaseLoaded = value;
                    NotifyPropertyChanged("IsBaseLoaded");
                }
            }
        }

        internal bool ValidateArray()
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (ArrayOfChars[i][j] == ' ')
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        #region Events Area

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}
