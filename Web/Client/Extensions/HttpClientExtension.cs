using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;

namespace Web.Client.Extensions
{
    public static class HttpClientExtension
    {
        public static async Task<T> GetJsonAsync<T>(this HttpContent content)
        {
            string text = await content.ReadAsStringAsync();
            if (!string.IsNullOrEmpty(text))
                return JsonConvert.DeserializeObject<T>(text);
            else
                return default(T);
        }

        public static string GetJsonString(this object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public static async Task<HttpClientResponse<T>> SendJsonResponseAsync<T>(this HttpClient httpClient, HttpMethod method, string url, T obj)
        {
            var msg = new HttpRequestMessage(method, url);
            msg.Content = new StringContent(obj.GetJsonString(), Encoding.UTF8, "application/json");
            var response = await httpClient.SendAsync(msg);

            int cooldown = 0;
            if (response.Headers.TryGetValues("cooldown", out IEnumerable<string> values))
            {
                var cooldownHeader = values.FirstOrDefault();

                if (cooldownHeader != null)
                    int.TryParse(cooldownHeader, out cooldown);
            }

            return new HttpClientResponse<T>(response.StatusCode, await response.Content.GetJsonAsync<T>(), cooldown);
        }

        public class HttpClientResponse<T>
        {
            public HttpClientResponse(HttpStatusCode statusCode, T content, int cooldown = 0)
            {
                StatusCode = statusCode;
                Content = content;
                Cooldown = cooldown;
            }

            public HttpStatusCode StatusCode { get; set; }
            public T Content { get; set; }
            public int Cooldown { get; set; }
        }
    }
}
