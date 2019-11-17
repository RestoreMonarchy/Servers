using Core.Models;
using Dapper;
using RestoreMonarchy.Database;
using System.Collections.Generic;
using System.Linq;

namespace Web.Server.Extensions.Database
{
    public static class DatabasePlayerExtension
    {
        public static Player CreatePlayer(this IDatabaseManager database, Player player)
        {
            string sql = "INSERT INTO dbo.Players (PlayerId, PlayerName, PlayerCountry, PlayerAvatar) OUTPUT inserted.* VALUES (@PlayerId, @PlayerName, @PlayerCountry, @PlayerAvatar);";

            using (var conn = database.Connection)
            {
                player = conn.QuerySingle<Player>(sql, player);
            }
            return player;
        }

        public static Player GetPlayer(this IDatabaseManager database, string playerId)
        {
            string sql = "SELECT * FROM dbo.Players WHERE PlayerId = @playerId;";

            using (var conn = database.Connection)
            {
                return conn.Query<Player>(sql, new { PlayerId = playerId }).FirstOrDefault();
            }
        }

        public static Dictionary<string, string> GetPlayersSearch(this IDatabaseManager database)
        {
            string sql = "SELECT PlayerId, PlayerName FROM dbo.Players;";

            using (var conn = database.Connection)
            {
                return conn.Query(sql).ToDictionary(x => (string)x.PlayerId, x=> (string)x.PlayerName);
            }
        }

        public static void PayPlayer(this IDatabaseManager database, string playerId, decimal amount)
        {
            string sql = "UPDATE dbo.Players SET Balance = Balance + @amount WHERE PlayerId = @playerId;";

            using (var conn = database.Connection)
            {
                conn.Execute(sql, new { playerId, amount });
            }
        }

        public static void UpdateLastActivity(this IDatabaseManager database, string playerId)
        {
            string sql = "UPDATE dbo.Players SET PlayerLastActivity = SYSDATETIME() WHERE PlayerId = @playerId;";

            using (var conn = database.Connection)
            {
                conn.Execute(sql, new { playerId });
            }
        }
    }
}
