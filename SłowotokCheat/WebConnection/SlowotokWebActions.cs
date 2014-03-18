using SłowotokCheat.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;
using SłowotokCheat.Utilities;
using System.Threading;

namespace SłowotokCheat.WebConnection
{
    public class SlowotokWebActions : IDisposable
    {
        public CookieAwareWebClient Client { get; private set; }

        public readonly string Email;
        private readonly string Password;
        private bool _isBusy = false;

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
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    while (_isBusy) { Thread.Sleep(100); } _isBusy = true;
                    Client.UploadValues("account/logon", "POST", new NameValueCollection()
                    {
                        {"Email", Email},
                        {"Password", Password}
                    });
                    _isBusy = false;
                });
            }
            catch (WebException)
            {
                OnConnectionError();
                return false;
            }

            if (Client.Cookies.Count > 0)
                return true;
            else
                return false;
        }

        public async Task<T> ReceiveStringAsync<T>(string downloadString) where T: class
        {
            try
            {
                return await Task.Factory.StartNew(() =>
                {
                    while (_isBusy) { Thread.Sleep(100); } _isBusy = true;
                    var response = Client.DownloadString(downloadString);
                    _isBusy = false;

                    return JsonConvert.DeserializeObject<T>(response);
                    
                });
            }
            catch (WebException)
            {
                OnConnectionError();
                return null;
            }
        }

        private void OnConnectionError()
        {
            if (ConnectionError != null)
                ConnectionError(this, new EventArgs());
        }

        public async Task<AnswersResponse> SendAnswers(List<WordRecord> foundWords)
        {
            var values = String.Join(",", foundWords.Select(x => x.Word.ToUpper()));
            string responseString = null;
            byte[] responseBytes;

            try
            {
                await Task.Factory.StartNew(() =>
                {
                    while (_isBusy) { Thread.Sleep(100); } _isBusy = true;
                    responseBytes = Client.UploadValues("play/answers", "POST", new NameValueCollection()
                    {
                        { "word_list", values}
                    });
                    _isBusy = false;

                    responseString = Encoding.UTF8.GetString(responseBytes);
                });
            }
            catch (WebException)
            {
                OnConnectionError();
                return null;
            }
            
            return Newtonsoft.Json.JsonConvert.DeserializeObject<AnswersResponse>(responseString);
        }

        public event ConnectionErrorEventHandler ConnectionError;

        public void Dispose()
        {
            if (Client != null) Client.Dispose();
            Client = null;
        }
    }
}
