using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SłowotokCheat.WebConnection
{
    public class SlowotokWebActions : IDisposable
    {
        public CookieAwareWebClient Client { get; private set; }

        public readonly string Email;
        private readonly string Password;

        public SlowotokWebActions(string _email, string _password)
        {
            Client = new CookieAwareWebClient();
            Client.BaseAddress = @"http://slowotok.pl/";
            Client.Headers.Add("User-Agent", "Mozilla/5.0 (Windows; U; Windows NT 6.1; en-GB; rv:1.9.2.12) Gecko/20101026 Firefox/3.6.12");
            Client.Encoding = Encoding.UTF8;

            Email = _email;
            Password = _password;
        }

        public void LogOn()
        {
            var loginData = new NameValueCollection();

            loginData.Add("Email", Email);
            loginData.Add("Password", Password);

            Client.UploadValuesCompleted += (sender, e) => MessageBox.Show(e.Result.ToString());
            Client.UploadValues("/account/logon", "POST", loginData);
        }

        public void Funtion()
        {
            SlowotokWebActions webActions = new SlowotokWebActions("kneefer@gmail.com", "dupa123");
            webActions.LogOn();
            string myBoard = webActions.Client.DownloadString("/play/board/");
            MessageBox.Show(myBoard);
        }

        public void Dispose()
        {
            Client.Dispose();
        }
    }
}
