using System;
using System.Net;

namespace Core.Utilities
{
    public class ApiWebClient : WebClient
    {
        public int Timeout { get; set; }

        public ApiWebClient()
        {
            Timeout = 2000;
        }

        protected override WebRequest GetWebRequest(Uri uri)
        {
            WebRequest w = base.GetWebRequest(uri);
            w.Timeout = 2;
            return w;
        }
    }
}
