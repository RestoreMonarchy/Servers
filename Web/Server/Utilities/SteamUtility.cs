using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Web.Server.Utilities
{
    public class SteamUtility
    {
        const string GetPlayerSummariesUrl = "http://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/";

        public static async Task<SteamPlayer> GetSteamPlayerAsync(string steamId, string steamApi)
        {
            HttpClient httpClient = new HttpClient();
            string content = await httpClient.GetStringAsync(GetPlayerSummariesUrl + $"?key={steamApi}&steamids={steamId}");
            List<SteamPlayer> players = JObject.Parse(content)["response"]["players"].ToObject<List<SteamPlayer>>();
            return players.FirstOrDefault();
        }

        public class SteamPlayer
        {
            public string steamid { get; set; }
            public int communityvisibilitystate { get; set; }
            public int profilestate { get; set; }
            public string personaname { get; set; }
            public int lastlogoff { get; set; }
            public int commentpermission { get; set; }
            public string profileurl { get; set; }
            public string avatar { get; set; }
            public string avatarmedium { get; set; }
            public string avatarfull { get; set; }
            public int personastate { get; set; }
            public string realname { get; set; }
            public string primaryclanid { get; set; }
            public int timecreated { get; set; }
            public int personastateflags { get; set; }
            public string loccountrycode { get; set; }
            public string locstatecode { get; set; }
        }
    }    
}
