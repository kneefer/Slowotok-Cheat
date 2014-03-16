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
        public ObservableCollection<WordRecord> FoundWords { get; set; }

        public MainPageViewModel()
        {
            IsBaseLoaded = false;
            FoundWords = new ObservableCollection<WordRecord>();
            InProgress = false;
            IsLoggedIn = false;
            AreAllWordsChecked = true;
            UserEmail = Properties.Settings.Default.LastUsedEmail;

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

        private string _informationBox;
        public string InformationBox
        {
            get { return _informationBox; }
            set
            {
                if (value != _informationBox)
                {
                    _informationBox = value;
                    NotifyPropertyChanged("InformationBox");
                }
            }
        }

        private TimeSpan _timeLeft;
        private TimeSpan _timeToGameEnd;
        private TimeSpan _timeToGetResults;

        public TimeSpan TimeLeft
        {
            get
            {
                if (_timeLeft < TimeSpan.Zero) return TimeSpan.Zero;
                return TimeSpan.FromSeconds(_timeLeft.Minutes * 60 + _timeLeft.Seconds);
            }
            set
            {
                if (value != _timeLeft)
                {
                    _timeLeft = value;
                    NotifyPropertyChanged("TimeLeft");
                }
            }
        }
        public TimeSpan TimeToGameEnd
        {
            get
            {
                if (_timeToGameEnd < TimeSpan.Zero) return TimeSpan.Zero;
                return TimeSpan.FromSeconds(_timeToGameEnd.Minutes * 60 + _timeToGameEnd.Seconds);
            }
            set
            {
                if (value != _timeToGameEnd)
                {
                    _timeToGameEnd = value;
                    NotifyPropertyChanged("TimeToGameEnd");
                }
            }
        }
        public TimeSpan TimeToGetResults
        {
            get
            {
                if (_timeToGetResults < TimeSpan.Zero) return TimeSpan.Zero;
                return TimeSpan.FromSeconds(_timeToGetResults.Minutes * 60 + _timeToGetResults.Seconds);
            }
            set
            {
                if (value != _timeToGetResults)
                {
                    _timeToGetResults = value;
                    NotifyPropertyChanged("TimeToGetResults");
                }
            }
        }

        private string _userEmail;
        public string UserEmail
        {
            get { return _userEmail; }
            set
            {
                if (value != _userEmail)
                {
                    _userEmail = value;
                    NotifyPropertyChanged("UserEmail");
                }
            }
        }

        private bool _isLoggedIn;
        public bool IsLoggedIn
        {
            get { return _isLoggedIn; }
            set
            {
                if (value != _isLoggedIn)
                {
                    _isLoggedIn = value;
                    NotifyPropertyChanged("IsLoggedIn");
                }
            }
        }

        private bool _areAllWordsChecked;
        public bool AreAllWordsChecked
        {
            get { return _areAllWordsChecked; }
            set
            {
                if (value != _areAllWordsChecked)
                {
                    _areAllWordsChecked = value;

                    foreach (var item in FoundWords)
                    {
                        item.IsSelected = value;
                    }

                    NotifyPropertyChanged("AreAllWordsChecked");
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
