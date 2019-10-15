using Core.Models;
using Dapper;
using System.Collections.Generic;
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

        public static PlayerBan GetPlayerBan(this DatabaseManager database, int banId)
        {
            string sql = "SELECT b.*, p.*, p2.* FROM dbo.PlayerBans AS b LEFT JOIN dbo.Players AS p ON p.PlayerId = b.PlayerId " +
                "LEFT JOIN dbo.Players AS p2 ON b.PunisherId = p2.PlayerId WHERE b.BanId = @banId;";

            using (var conn = database.connection)
            {
                return conn.Query<PlayerBan, Player, Player, PlayerBan>(sql,
                (b, p, p2) =>
                {
                    b.Player = p;
                    b.Punisher = p2;
                    return b;
                }, new { banId }, splitOn: "PlayerId,PlayerId").FirstOrDefault();
            }
        }

        public static List<PlayerBan> GetActivePlayerBans(this DatabaseManager database)
        {
            string sql = "SELECT b.*, p.*, p2.* FROM dbo.PlayerBans AS b LEFT JOIN dbo.Players AS p ON p.PlayerId = b.PlayerId " +
                "LEFT JOIN dbo.Players AS p2 ON b.PunisherId = p2.PlayerId WHERE b.SendFlag = 0;";

            using (var conn = database.connection)
            {
                return conn.Query<PlayerBan, Player, Player, PlayerBan>(sql,
                (b, p, p2) =>
                {
                    b.Player = p;
                    b.Punisher = p2;
                    return b;
                }, splitOn: "PlayerId,PlayerId").ToList();
            }
        }

        public static Player GetPlayer(this DatabaseManager database, string playerId)
        {
            string sql = "SELECT p.*, b.* FROM dbo.Players AS p LEFT JOIN dbo.PlayerBans AS b ON b.PlayerId = p.PlayerId WHERE p.PlayerId = @PlayerId;";
            Player player = null;

            using (var conn = database.connection)
            {
                conn.Query<Player, PlayerBan, Player>(sql,
                    (p, b) =>
                    {
                        if (player == null)
                            player = p;
                        if (player.PlayerBans == null)
                            player.PlayerBans = new List<PlayerBan>();

                        if (b != null)
                            player.PlayerBans.Add(b);
                        return p;
                    }, new { PlayerId = playerId }, splitOn: "BanId");
            }
            return player;
        }

        public static void SetPlayerBanFlag(this DatabaseManager database, int banId, bool flag)
        {
            string sql = "UPDATE dbo.PlayerBans SET SendFlag = @SendFlag WHERE BanId = @BanId;";
            using (var conn = database.connection)
            {
                conn.Execute(sql, new { SendFlag = flag, BanId = banId });
            }
        }
    }
}
