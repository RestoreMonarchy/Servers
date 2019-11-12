using Newtonsoft.Json;
using Rocket.Core.Logging;
using System.Net;

namespace RestoreMonarchy.Audit.Utilities
{
    public static class AuditUtility
    {
        public static T DownloadJson<T>(this WebClient wc, string url)
        {
            T obj = default(T);
            string body = null;

            try
            {
                using (wc)
                {
                    body = wc.DownloadString(url);
                }
            } catch (WebException e)
            {
                Logger.LogException(e);
            }

            try
            {
                obj = JsonConvert.DeserializeObject<T>(body);
            } catch (JsonException e)
            {
                Logger.LogException(e);
            }

            return obj;
        }
    }
}
