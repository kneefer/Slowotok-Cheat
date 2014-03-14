using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SłowotokCheat
{
    public class WebActions
    {
        public CookieAwareWebClient Client { get; private set; }

        public string Email { get; private set; }
        private readonly string Password;

        public WebActions(string _email, string _password)
        {
            Client = new CookieAwareWebClient();
            Client.BaseAddress = @"http://slowotok.pl/account/logon/";

            Email = _email;
            Password = _password;
        }

        public void LogOn()
        {
            var loginData = new NameValueCollection();

            loginData.Add("Email", Email);
            loginData.Add("Password", Password);

            Client.UploadValuesCompleted += (sender, e) => MessageBox.Show(e.Result.ToString());
            Client.UploadValues("login.php", "POST", loginData);
        }   
    }
}
