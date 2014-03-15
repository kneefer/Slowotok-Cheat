using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
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
            Client.BaseAddress = @"http://slowotok.pl";
            Client.Headers.Add("User-Agent", "Mozilla/5.0 (Windows; U; Windows NT 6.1; en-GB; rv:1.9.2.12) Gecko/20101026 Firefox/3.6.12");
            Client.Encoding = Encoding.UTF8;

            Email = _email;
            Password = _password;
        }

        public async Task<bool> LogOn()
        {
            var loginData = new NameValueCollection();

            loginData.Add("Email", Email);
            loginData.Add("Password", Password);

            try
            {
                await Task.Factory.StartNew(() =>
                {
                    Client.UploadValues("account/logon", "POST", loginData);
                });
            }
            catch (WebException)
            {
                MessageBox.Show("Connection error!");
            }

            if (Client.Cookies.Count > 0)
                return true;
            else
                return false;
        }

        public void Dispose()
        {
            if (Client != null) Client.Dispose();
            Client = null;
        }
    }
}
