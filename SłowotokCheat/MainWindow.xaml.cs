using SłowotokCheat.Models;
using SłowotokCheat.Utilities;
using SłowotokCheat.WebConnection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Security.Cryptography;
using System.Threading;

namespace SłowotokCheat
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Properties and Constants Area

        public const string DICTIONARY_FILENAME = "slowa.txt";

        private MainPageViewModel vm = new MainPageViewModel();
        private char[,] arrayToProcess;
        private System.Windows.Forms.NotifyIcon ni = new System.Windows.Forms.NotifyIcon();
        public GameManagement GameOps { get; set; }
        public HashSet<string> Dictionary { get; set; }
        

        #endregion

        public MainWindow()
        {
            InitializeComponent();
            DataContext = vm;
            Dictionary = new HashSet<string>();

            ni.Icon = new System.Drawing.Icon("favicon.ico");
            ni.Visible = true;
            ni.Text = "Słowotok Cheat";

            ni.BalloonTipClicked += (sender, args) =>
            {
                this.Show();
                this.WindowState = WindowState.Normal;
            };

            ni.Click += (sender, args) =>
            {
                this.Show();
                this.WindowState = WindowState.Normal;
            };
        }

        protected override void OnStateChanged(EventArgs e)
        {
            if (WindowState == System.Windows.WindowState.Minimized)
            {
                this.Hide();
                ni.ShowBalloonTip(3000, "Słowotok Cheat", "The program is hidden in tray but is still working!", System.Windows.Forms.ToolTipIcon.Info);
            }

            base.OnStateChanged(e);
        }

        #region Generator Region
        private async Task BeginProcessing()
        {
            if (vm.InProgress) return;

            vm.InformationBox = "Generating the words...";
            vm.InProgress = true;
            vm.FoundWords.Clear();

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

            vm.InProgress = false;
        }

        private void generateWords(char[,] array, string word, int x, int y, int recursLvl = 16)
        {
            char[,] _array = ((char[,])array.Clone()); // cloning the array
            _array[x, y] = '_';

            var possibilities = isMovePossible(_array, x, y);

            if (word.Length >= 3 && Dictionary.Contains(word))
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    if (vm.FoundWords.FirstOrDefault(z => z.Word.Equals(word)) == null
                        && (GameOps != null ? GameOps.CurrentBoard.Hashs.Contains(word.CalculateMD5()) : true))
                    {
                        vm.FoundWords.AddSorted(
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
                    generateWords(_array, word + _array[nextMove.X, nextMove.Y], nextMove.X, nextMove.Y, recursLvl-1);
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
                    if (a >= 0 && a < 4 && b >= 0 && b < 4 && array[a,b] != '_')
                        possibilities.Add(new IntPoint(a, b));
                }
            }
            
            return possibilities;
        }

        #endregion

        #region Login/Logout Click Events

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (!vm.IsBaseLoaded)
            {
                MessageBox.Show("Load the base first!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            vm.InProgress = true;
            vm.InformationBox = "Logging in...";

            GameOps = new GameManagement();
            GameOps.WebActions = new SlowotokWebActions(vm.UserEmail, passwordBox.Password);

            if (await GameOps.WebActions.LogOn())
            {
                vm.IsLoggedIn = true;
                GameOps.BoardChanged += GameOps_BoardChanged;
                GameOps.SendAnswerGotPossible += GameOps_SendAnswerGotPossible;
                GameOps.PropertyChanged += GameOps_PropertyChanged;
                vm.InProgress = false;
                GameOps.StartAutomation();
            }
            else
            {
                vm.InProgress = false;
                MessageBox.Show("Incorrect email or password!", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            vm.IsLoggedIn = false;
            GameOps.StopAutomation();
            GameOps.BoardChanged -= GameOps_BoardChanged;
            GameOps.PropertyChanged -= GameOps_PropertyChanged;
            GameOps.SendAnswerGotPossible -= GameOps_SendAnswerGotPossible;

            GameOps.Dispose();
            GameOps = null;
        }

        #endregion

        void GameOps_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            GameManagement gm = sender as GameManagement;

            switch (e.PropertyName)
            {
                case "TimeLeft": vm.TimeLeft = gm.TimeLeft; return;
                case "TimeToGameEnd": vm.TimeToGameEnd = gm.TimeToGameEnd; return;
                case "TimeToGetResults": vm.TimeToGetResults = gm.TimeToGetResults; return;
                default: return;
            }
        }

        private async void GameOps_SendAnswerGotPossible(object sender, EventArgs e)
        {
            if (vm.InProgress) return;

            vm.InProgress = true;
            vm.InformationBox = "Receiving results...";

            var response = await GameOps.SendAnswers(vm.FoundWords.ToList().Where(x => x.IsSelected).ToList());

            var info = "You gained " + response.Answers.Where(x => x.Found).Sum(y => (y.Word.Length - 2).Pow(2)).ToString()
                                + "/" + GameOps.CurrentBoard.Points + " points!";

            if (WindowState == System.Windows.WindowState.Minimized)
            {
                ni.ShowBalloonTip(3000, "Słowotok Cheat", info, System.Windows.Forms.ToolTipIcon.Info);
            }
            else
            {
                vm.InformationBox = info;
                await Task.Delay(5000);
            }

            vm.InProgress = false;
        }

        private void GameOps_BoardChanged(object sender, BoardEventArgs e)
        {
            vm.ArrayOfChars = e.NewBoard.Letters.ConvertToJaggedArray(4, 4);
            validateRequirementsToProcessing(sender, new RoutedEventArgs());
        }


        private async void LoadTheBase_Loaded(object sender, RoutedEventArgs e)
        {
            if (vm.InProgress) return;
            vm.InProgress = true;
            vm.InformationBox = "Loading the base (it may take a while)...";

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

                vm.IsBaseLoaded = true;
                grid.Focus();
                this.Width += 1;
                passwordBox.Focus();
            }
            catch (FileNotFoundException ex)
            {
                MessageBox.Show(String.Format("Not found dictionary file: \"{0}\" in program directory! Place the file and restart program!", ex.FileName) ,
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("Error: {0}", ex), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                vm.InProgress = false;
            }
        }

        private async void validateRequirementsToProcessing(object sender, RoutedEventArgs e)
        {
            if (!vm.IsBaseLoaded)
            {
                MessageBox.Show("Load the base first!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!vm.ValidateArray())
            {
                MessageBox.Show("Fill all of the fields first!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            this.arrayToProcess = vm.ArrayOfChars.ConvertTo2DArray(4, 4);

            await BeginProcessing();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Properties.Settings.Default.LastUsedEmail = vm.UserEmail;
            Properties.Settings.Default.Save();
            ni.Dispose();
        }
    }
}
