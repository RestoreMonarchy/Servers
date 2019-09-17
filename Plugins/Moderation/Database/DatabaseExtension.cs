using RestoreMonarchy.Moderation.Models;
using System;
using System.Collections.Generic;
using Dapper;
using System.Linq;
using System.Threading.Tasks;

namespace RestoreMonarchy.Moderation.Database
{
    public static class DatabaseExtension
    {
        public static Player GetPlayer(this DatabaseManager manager, ulong steamId)
        {
            string sql = "SELECT p.*, b.* FROM Players AS p LEFT JOIN Bans AS b ON b.PlayerId = p.PlayerId WHERE p.PlayerId = @PlayerId;";
            Player player = null;
            
            using (var conn = manager.Connection)
            {
                conn.Query<Player, Ban, Player>(sql, 
                    (p, b) =>
                    {
                        if (player == null)
                            player = p;
                        if (player.Bans == null)
                            player.Bans = new List<Ban>();

                        player.Bans.Add(b);
                        return p;
                    }, new { PlayerId = steamId }, splitOn: "BanId");  
            }

            return player;
        }        

        public static Player CreatePlayer(this DatabaseManager manager, Player player)
        {
            string sql = "INSERT INTO Players (PlayerId, PlayerName, PlayerCountry, PlayerCreated) VALUES (@PlayerId, @PlayerName, @PlayerCountry, @PlayerCreated);";

            using (var conn = manager.Connection)
            {
                conn.Execute(sql, player);
            }

            return player;
        }

        public static void InsertBan(this DatabaseManager manager, Ban ban)
        {
            string sql = "INSERT INTO Bans (PlayerId, PunisherId, BanReason, BanDuration, BanCreated) VALUES (@PlayerId, @PunisherId, @BanReason, @BanDuration, @BanCreated);";

            using (var conn = manager.Connection)
            {
                conn.Execute(sql, ban);
            }
        }

        public static void RemoveBan(this DatabaseManager manager, int banId)
        {
            string sql = "DELETE FROM Bans WHERE BanId = @BanId;";

            using (var conn = manager.Connection)
            {
                conn.Execute(sql, new { BanId = banId });
            }
        }

        public static void SetFlag(this DatabaseManager manager, int banId)
        {
            string sql = "UPDATE Bans SET SendFlag = true WHERE BanId = @BanId;";

            using (var conn = manager.Connection)
            {
                conn.Execute(sql, new { BanId = banId });
            }
        }
    }
}
