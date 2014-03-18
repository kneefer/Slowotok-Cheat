using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SłowotokCheat.WebConnection
{
    public class CookieAwareWebClient : WebClient
    {
        public CookieContainer Cookies { get; private set; }

        public CookieAwareWebClient()
        {
            Cookies = new CookieContainer();
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            WebRequest request = base.GetWebRequest(address);
            if (request is HttpWebRequest)
            {
                (request as HttpWebRequest).CookieContainer = Cookies;
            }
            request.Timeout = 2000;
            return request;
        }
    }
}
