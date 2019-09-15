using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Core.Plugins.Models
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
