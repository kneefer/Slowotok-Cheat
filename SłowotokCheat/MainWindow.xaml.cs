using GalaSoft.MvvmLight.Messaging;
using SłowotokCheat.ViewModel;
using System;
using System.IO;
using System.Windows;

namespace SłowotokCheat
{
    public partial class MainWindow : Window
    {
        private System.Windows.Forms.NotifyIcon _ni = new System.Windows.Forms.NotifyIcon();
        
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
                    _ni.ShowBalloonTip(3000, "Słowotok Cheat", msg, System.Windows.Forms.ToolTipIcon.Info);
                }
                else
                {
                    Messenger.Default.Send<string>(msg, "toVM"); // send to the MainViewModel
                }
            });
        }

        private void ConfigureBaloonNotifications()
        {
            using (var stream = Application.GetResourceStream(new Uri(
                "pack://application:,,,/SłowotokCheat;component/Resources/favicon.ico"
            )).Stream)
            {
                _ni.Icon = new System.Drawing.Icon(stream);
            }

            _ni.Visible = true;
            _ni.Text = "Słowotok Cheat";

            _ni.BalloonTipClicked += ShowBaloonTip;
            _ni.Click += ShowBaloonTip;
        }

        private void ShowBaloonTip(object sender, EventArgs e)
        {
            this.Show();
            this.WindowState = WindowState.Normal;
        }

        protected override void OnStateChanged(EventArgs e)
        {
            if (WindowState == System.Windows.WindowState.Minimized)
            {
                this.Hide();
                _ni.ShowBalloonTip(3000, "Słowotok Cheat", "The program is hidden in tray but is still working!", System.Windows.Forms.ToolTipIcon.Info);
            }

            base.OnStateChanged(e);
        }

        //
        // Events which was hard to implement as RelayCommand (leaved for the clarity)

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

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);

            Properties.Settings.Default.LastUsedEmail = emailBox.Text;
            Properties.Settings.Default.Save();
            _ni.Dispose();
        }
    }
}
