using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using SłowotokCheat.Models;
using System;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace SłowotokCheat.ViewModel
{
    public partial class MainViewModel : ViewModelBase
    {
        /////////////////////////////////////////////////
        // PRIVATE PARTS OF OBSERVABLE PROPERTIES AREA //
        /////////////////////////////////////////////////
        private ObservableCollection<WordRecord> _foundWords;
        private char[][] _arrayOfChars;
        private bool _areAllWordsChecked;
        private bool _isBaseLoaded;

        private bool _inProgress;
        private bool _areConnectionProblems;
        private string _informationBox;

        private TimeSpan _timeLeft;
        private TimeSpan _timeToGameEnd;
        private TimeSpan _timeToGetResults;

        private string _userEmail;
        private bool _isLoggedIn;

        ////////////////////////////////////////////////
        // PUBLIC PARTS OF OBSERVABLE PROPERTIES AREA //
        ////////////////////////////////////////////////
        public ObservableCollection<WordRecord> FoundWords
        {
            get { return _foundWords; }
            set
            {
                if (value != _foundWords)
                {
                    _foundWords = value;
                    RaisePropertyChanged("FoundWords");
                }
            }
        }
        public char[][] ArrayOfChars
        {
            get { return _arrayOfChars; }
            set
            {
                if (value != _arrayOfChars)
                {
                    _arrayOfChars = value;
                    RaisePropertyChanged("ArrayOfChars");
                }
            }
        }
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

                    RaisePropertyChanged("AreAllWordsChecked");
                }
            }
        }
        public bool IsBaseLoaded
        {
            get { return _isBaseLoaded; }
            set
            {
                if (value != _isBaseLoaded)
                {
                    _isBaseLoaded = value;
                    RaisePropertyChanged("IsBaseLoaded");
                }
            }
        }
        public bool InProgress
        {
            get { return _inProgress; }
            set
            {
                if (value != _inProgress)
                {
                    _inProgress = value;
                    RaisePropertyChanged("InProgress");
                }
            }
        }
        public bool AreConnectionProblems
        {
            get { return _areConnectionProblems; }
            set
            {
                if (value != _areConnectionProblems)
                {
                    _areConnectionProblems = value;
                    RaisePropertyChanged("AreConnectionProblems");
                }
            }
        }
        public string InformationBox
        {
            get { return _informationBox; }
            set
            {
                if (value != _informationBox)
                {
                    _informationBox = value;
                    RaisePropertyChanged("InformationBox");
                }
            }
        }
        public TimeSpan TimeLeft
        {
            get
            {
                if (_timeLeft < TimeSpan.Zero) return TimeSpan.Zero;
                return _timeLeft;
            }
            set
            {
                if (value != _timeLeft)
                {
                    _timeLeft = value;
                    RaisePropertyChanged("TimeLeft");
                }
            }
        }
        public TimeSpan TimeToGameEnd
        {
            get
            {
                if (_timeToGameEnd < TimeSpan.Zero) return TimeSpan.Zero;
                return _timeToGameEnd;
            }
            set
            {
                if (value != _timeToGameEnd)
                {
                    _timeToGameEnd = value;
                    RaisePropertyChanged("TimeToGameEnd");
                }
            }
        }
        public TimeSpan TimeToGetResults
        {
            get
            {
                if (_timeToGetResults < TimeSpan.Zero) return TimeSpan.Zero;
                return _timeToGetResults;
            }
            set
            {
                if (value != _timeToGetResults)
                {
                    _timeToGetResults = value;
                    RaisePropertyChanged("TimeToGetResults");
                }
            }
        }
        public string UserEmail
        {
            get { return _userEmail; }
            set
            {
                if (value != _userEmail)
                {
                    _userEmail = value;
                    RaisePropertyChanged("UserEmail");
                }
            }
        }
        public bool IsLoggedIn
        {
            get { return _isLoggedIn; }
            set
            {
                if (value != _isLoggedIn)
                {
                    _isLoggedIn = value;
                    RaisePropertyChanged("IsLoggedIn");
                }
            }
        }

        ///////////////////////////////
        // MVVM LIGHT COMMANDS  AREA //
        ///////////////////////////////

        public RelayCommand<PasswordBox> LogInCommand { get; private set; }
        public RelayCommand LogOutCommand { get; private set; }
        public RelayCommand FindWordsCommand { get; private set; }

        private void InitializeCommands()
        {
            LogInCommand = new RelayCommand<PasswordBox>((passBox) => LogIn(passBox));
            LogOutCommand = new RelayCommand(() => LogOut());
            FindWordsCommand = new RelayCommand(() => FindWords());
        }
    }
}