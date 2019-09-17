using Newtonsoft.Json.Linq;
using RestoreMonarchy.Moderation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RestoreMonarchy.Moderation.Extensions
{
    public static class PlayerHelper
    {
        public static string DownloadCountry(string ip)
        {
            string json;

            using (WebClient wc = new WebClient())
            {
                json = wc.DownloadString("http://ip-api.com/json/" + ip);
            }

            JObject response = JObject.Parse(json);
            string countryCode;

            if ((string)response["status"] == "success")
                countryCode = (string)response["countryCode"];
            else
                countryCode = null;

            return countryCode;
        }

        public static string DownloadName(ulong steamId)
        {
            string json;
            using (WebClient wc = new WebClient())
            {
                json = wc.DownloadString("http://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key=29B58D603971129885DDF421956C4FEA&steamids=" + steamId);
            }

            JObject response = JObject.Parse(json);

            return response?["response"]?["players"]?[0]?["personaname"]?.ToString() ?? null;
        }

        public static bool IsBanned(this Player player, out Ban ban)
        {
            ban = null;
            if (player?.Bans != null)
            {
                foreach (Ban b in player.Bans)
                {
                    if (b.BanDuration.HasValue)
                    {
                        if (DateTime.Now < b.BanCreated.AddSeconds(b.BanDuration.Value))
                        {
                            ban = b;
                            return true;
                        }
                    }
                    else
                    {
                        ban = b;
                        return true;
                    }
                }
            }

            return false;
        }

        public static bool IsExpired(this Ban ban)
        {
            if (ban.BanDuration.HasValue)
            {
                return DateTime.Now > ban.BanCreated.AddSeconds(ban.BanDuration.Value);
            }

            return false;            
        }

        public static string GetTimeLeft(this Ban ban)
        {
            TimeSpan span = DateTime.Now - ban.BanCreated;

            return span.ToPrettyTime();
        }
    }
}
