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
using SłowotokCheat.ViewModel;
using GalaSoft.MvvmLight.Messaging;

namespace SłowotokCheat
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Properties and Constants Area

        public const string DICTIONARY_FILENAME = "slowa.txt";
        private System.Windows.Forms.NotifyIcon ni = new System.Windows.Forms.NotifyIcon();
        public GameManagement GameOps { get; set; }
        public HashSet<string> Dictionary { get; set; }
        

        #endregion

        public MainWindow()
        {
            InitializeComponent();
            ConfigureBaloonNotifications();
            RegisterBaloonMessagesFromViewModel();
        }

        private void RegisterBaloonMessagesFromViewModel()
        {
            Messenger.Default.Register<string>(this, "toWindow" , msg =>
            {
                if (WindowState == System.Windows.WindowState.Minimized)
                {
                    ni.ShowBalloonTip(3000, "Słowotok Cheat", msg, System.Windows.Forms.ToolTipIcon.Info);
                }
                else
                {
                    Messenger.Default.Send<string>(msg, "toVM"); // send to the MainViewModel
                }
            });
        }

        private void ConfigureBaloonNotifications()
        {
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

        //
        // Events which was hard to implement as RelayCommand (leaved for the clarity)

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Properties.Settings.Default.LastUsedEmail = emailBox.Text;
            Properties.Settings.Default.Save();
            ni.Dispose();
        }



        private void FindWords_EnterPressed(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as MainViewModel;
            if (vm != null)
            {
                vm.FindWords();
            }
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as MainViewModel;
            if (vm != null)
            {
                await vm.LoadTheBase();
            }
        }
    }
}
