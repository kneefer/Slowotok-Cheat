using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using SłowotokCheat.Models;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using SłowotokCheat.Utilities;
using System.Threading.Tasks;
using SłowotokCheat.WebConnection;
using System.Collections.Generic;
using System.Windows.Threading;
using System.Linq;
using System.IO;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Messaging;

namespace SłowotokCheat.ViewModel
{
    public partial class MainViewModel
    {
        public const string DICTIONARY_FILENAME = "slowa.txt";
        public GameManagement GameOps { get; set; }
        public HashSet<string> Dictionary { get; set; }

        public MainViewModel()
        {
            InitializeCommands();

            RegisterBaloonMessagesFromMainWindow();

            // Initialized every property to clear view on
            // all available properties in viewmodel (partial)

            FoundWords = new ObservableCollection<WordRecord>();
            ArrayOfChars = new char[4][]{
                new char[4] {' ', ' ', ' ', ' '},
                new char[4] {' ', ' ', ' ', ' '},
                new char[4] {' ', ' ', ' ', ' '},
                new char[4] {' ', ' ', ' ', ' '}
            };
            AreAllWordsChecked = true;
            IsBaseLoaded = false;
            
            InProgress = false;
            AreConnectionProblems = false;
            InformationBox = String.Empty;

            TimeLeft = TimeSpan.Zero;
            TimeToGameEnd = TimeSpan.Zero;
            TimeToGetResults = TimeSpan.Zero;

            UserEmail = Properties.Settings.Default.LastUsedEmail;
            IsLoggedIn = false;

            Dictionary = new HashSet<string>();
        }

        private void RegisterBaloonMessagesFromMainWindow()
        {
            Messenger.Default.Register<string>(this, "toVM" , NotifyUser);
        }

        private async void NotifyUser(string msg)
        {
            InformationBox = msg;
            await Task.Delay(5000);
            InProgress = false;
        }

        #region Generator Region
        private async Task BeginProcessing(char[,] arrayToProcess)
        {
            if (InProgress) return;

            InformationBox = "Generating the words...";
            InProgress = true;
            FoundWords.Clear();

            await Task.Factory.StartNew(() =>
            {
                Parallel.For(0, 4, x =>
                {
                    for (int y = 0; y < 4; y++)
                    {
                        generateWords(arrayToProcess, arrayToProcess[x, y].ToString(), x, y);
                    }
                });
            });

            InProgress = false;
        }

        private void generateWords(char[,] array, string word, int x, int y, int recursLvl = 16)
        {
            char[,] _array = ((char[,])array.Clone()); // cloning the array
            _array[x, y] = '_';

            var possibilities = isMovePossible(_array, x, y);

            if (word.Length >= 3 && Dictionary.Contains(word))
            {
                DispatchService.Invoke(new Action(() =>
                {
                    if (FoundWords.FirstOrDefault(z => z.Word.Equals(word)) == null
                        && (GameOps != null ? GameOps.CurrentBoard.Hashs.Contains(word.CalculateMD5()) : true))
                    {
                        FoundWords.AddSorted(
                            new WordRecord() { Word = word, Length = word.Length },
                            (a, b) => b.Length.CompareTo(a.Length)
                        );
                    }
                }));
            }


            if (possibilities.Count != 0 && recursLvl != 1)
            {
                foreach (var nextMove in possibilities)
                {
                    // the recursion
                    generateWords(_array, word + _array[nextMove.X, nextMove.Y], nextMove.X, nextMove.Y, recursLvl - 1);
                }
            }
        }

        private List<IntPoint> isMovePossible(char[,] array, int x, int y)
        {
            var possibilities = new List<IntPoint>();

            for (int a = (x - 1); a <= (x + 1); a++)
            {
                for (int b = (y - 1); b <= (y + 1); b++)
                {
                    if (a >= 0 && a < 4 && b >= 0 && b < 4 && array[a, b] != '_')
                        possibilities.Add(new IntPoint(a, b));
                }
            }

            return possibilities;
        }

        #endregion

        private async void LogIn(PasswordBox passBox)
        {
            if (!IsBaseLoaded)
            {
                MessageBox.Show("Load the base first!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            InProgress = true;
            InformationBox = "Logging in...";

            GameOps = new GameManagement();
            GameOps.WebActions = new SlowotokWebActions(UserEmail, passBox.Password);

            if (await GameOps.WebActions.LogOn())
            {
                IsLoggedIn = true;
                GameOps.BoardChanged += GameOps_BoardChanged;
                GameOps.SendAnswerGotPossible += GameOps_SendAnswerGotPossible;
                GameOps.PropertyChanged += GameOps_PropertyChanged;
                GameOps.WebActions.ConnectionError += WebActions_ConnectionError;
                InProgress = false;
                GameOps.StartAutomation();
            }
            else
            {
                InProgress = false;
                MessageBox.Show("Incorrect email or password!", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private async void WebActions_ConnectionError(object sender, EventArgs e)
        {
            AreConnectionProblems = true;
            await Task.Delay(10000);
            AreConnectionProblems = false;
        }

        private void GameOps_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            GameManagement gm = sender as GameManagement;

            switch (e.PropertyName)
            {
                case "TimeLeft": TimeLeft = gm.TimeLeft; return;
                case "TimeToGameEnd": TimeToGameEnd = gm.TimeToGameEnd; return;
                case "TimeToGetResults": TimeToGetResults = gm.TimeToGetResults; return;
                default: return;
            }
        }

        private async void GameOps_SendAnswerGotPossible(object sender, EventArgs e)
        {
            if (InProgress) return;

            InProgress = true;
            InformationBox = "Receiving results...";

            AnswersResponse response;
            if ((response = await GameOps.SendAnswers(FoundWords.ToList().Where(x => x.IsSelected).ToList())) != null)
            {
                var info = "You gained " + response.Answers.Where(x => x.Found).Sum(y => (y.Word.Length - 2).Pow(2)).ToString()
                                    + "/" + GameOps.CurrentBoard.Points + " points!";

                Messenger.Default.Send<string>(info, "toWindow"); // send to the MainWindow
            }
        }

        private void GameOps_BoardChanged(object sender, BoardEventArgs e)
        {
            ArrayOfChars = e.NewBoard.Letters.ConvertToJaggedArray(4, 4);
            FindWords();
        }

        private void LogOut()
        {
            IsLoggedIn = false;
            GameOps.StopAutomation();
            GameOps.BoardChanged -= GameOps_BoardChanged;
            GameOps.PropertyChanged -= GameOps_PropertyChanged;
            GameOps.SendAnswerGotPossible -= GameOps_SendAnswerGotPossible;
            GameOps.WebActions.ConnectionError -= WebActions_ConnectionError;

            GameOps.Dispose();
            GameOps = null;
        }

        public async void FindWords()
        {
            if (!IsBaseLoaded)
            {
                MessageBox.Show("Load the base first!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!ValidateArray())
            {
                MessageBox.Show("Fill all of the fields first!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            await BeginProcessing(ArrayOfChars.ConvertTo2DArray(4, 4));
        }

        public async Task LoadTheBase()
        {
            if (InProgress) return;
            InProgress = true;
            InformationBox = "Loading the base (it may take a while)...";

            try
            {
                using (StreamReader file = new StreamReader(DICTIONARY_FILENAME))
                {
                    string line;

                    while ((line = await file.ReadLineAsync()) != null)
                    {
                        Dictionary.Add(line);
                    }
                }

                IsBaseLoaded = true;
            }
            catch (FileNotFoundException ex)
            {
                MessageBox.Show(String.Format("Not found dictionary file: \"{0}\" in program directory! Place the file and restart program!", ex.FileName),
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("Error: {0}", ex), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                InProgress = false;
            }
        }

        private bool ValidateArray()
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
    }
}