using Core.Models;
using Dapper;
using System.Linq;

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
    }
}
