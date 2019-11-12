using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;

namespace Web.Client.Utilities
{
    public static class HttpClientExtensions
    {
        public static async Task<T> GetJsonAsync<T>(this HttpContent content)
        {
            string text = await content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(text);
        }
    }
}
