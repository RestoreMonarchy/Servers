using Core.Models;
using Dapper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Web.Server.Utilities.Database
{
    public static class DatabasePlayerExtension
    {
        public static Player CreatePlayer(this DatabaseManager database, Player player)
        {
            string sql = "INSERT INTO dbo.Players (PlayerId, PlayerName, PlayerCountry, PlayerAvatar) OUTPUT inserted.* VALUES (@PlayerId, @PlayerName, @PlayerCountry, @PlayerAvatar);";

            using (var conn = database.connection)
            {
                player = conn.QuerySingle<Player>(sql, player);
            }
            return player;
        }

        public static Player GetPlayer(this DatabaseManager database, string playerId)
        {
            string sql = "SELECT * FROM dbo.Players WHERE PlayerId = @playerId;";

            using (var conn = database.connection)
            {
                return conn.Query<Player>(sql, new { PlayerId = playerId }).FirstOrDefault();
            }
        }

        public static Dictionary<string, string> GetPlayersSearch(this DatabaseManager database)
        {
            string sql = "SELECT PlayerId, PlayerName FROM dbo.Players;";

            using (var conn = database.connection)
            {
                return conn.Query(sql).ToDictionary(x => (string)x.PlayerId, x=> (string)x.PlayerName);
            }
        }

        public static void PayPlayer(this DatabaseManager database, string playerId, decimal amount)
        {
            string sql = "UPDATE dbo.Players SET Balance = Balance + @amount WHERE PlayerId = @playerId;";

            using (var conn = database.connection)
            {
                conn.Execute(sql, new { playerId, amount });
            }
        }

        public static void UpdateLastActivity(this DatabaseManager database, string playerId)
        {
            string sql = "UPDATE dbo.Players SET PlayerLastActivity = SYSDATETIME() WHERE PlayerId = @playerId;";

            using (var conn = database.connection)
            {
                conn.Execute(sql, new { playerId });
            }
        }

        public static async Task InitializePlayerAsync(this DatabaseManager database, CookieValidatePrincipalContext context)
        {
            string steamId = context.Principal.FindFirst(ClaimTypes.NameIdentifier).Value.Substring(37);
            string ip = context.Request.HttpContext.Connection.RemoteIpAddress.ToString();

            Player player = await database.GetInitializedPlayerAsync(steamId, ip);

            List<Claim> claims = new List<Claim>();

            claims.Add(new Claim(ClaimTypes.Name, player.PlayerId));
            claims.Add(new Claim(ClaimTypes.Role, player.Role));

            context.Principal.AddIdentity(new ClaimsIdentity(claims, "DefaultAuth"));                       
        }

        public static async Task<Player> GetInitializedPlayerAsync(this DatabaseManager database, string steamId, string ip)
        {
            Player player = database.GetPlayer(steamId);

            if (player == null)
            {
                HttpClient httpClient = new HttpClient();
                string url = "http://ip-api.com/json/" + ip;
                string content = await httpClient.GetStringAsync(url);
                JObject obj = JObject.Parse(content);

                string countryCode = null;
                if (obj["status"].ToString() == "success")
                {
                    countryCode = obj["countryCode"].ToString();
                }

                var steamPlayer = await SteamUtility.GetSteamPlayerAsync(steamId, database.configuration["SteamAPI"]);

                if (steamPlayer != null)
                {
                    player = new Player(steamId, steamPlayer.personaname, countryCode);
                    player.PlayerAvatar = await httpClient.GetByteArrayAsync(steamPlayer.avatarfull);
                    player = database.CreatePlayer(player);
                }

                await database.InvokePlayerCreatedAsync(player);
            }

            return player;
        }
    }
}
