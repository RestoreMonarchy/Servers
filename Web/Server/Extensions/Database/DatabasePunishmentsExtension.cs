using Core.Models;
using Dapper;
using RestoreMonarchy.Database;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Web.Server.Extensions.Database
{
    public static class DatabasePunishmentsExtension
    {
        public static int CreatePunishment(this IDatabaseManager database, PlayerPunishment punishment)
        {
            string sql = "INSERT INTO dbo.PlayerPunishments (PlayerId, PunisherId, Category, Reason, ExpiryDate, CreateDate) OUTPUT INSERTED.PunishmentId " +
                "VALUES (@PlayerId, @PunisherId, @Category, @Reason, @ExpiryDate, @CreateDate);";

            using (var conn = database.Connection)
            {
                return conn.ExecuteScalar<int>(sql, punishment);
            }
        }
        
        public static List<PlayerPunishment> GetPunishments(this IDatabaseManager database)
        {
            string sql = "SELECT p.*, p1.*, p2.* FROM dbo.PlayerPunishments p JOIN dbo.Players p1 ON p.PlayerId = p1.PlayerId JOIN dbo.Players p2 ON p.PunisherId = p2.PlayerId;";

            using (var conn = database.Connection)
            {
                return conn.Query<PlayerPunishment, Player, Player, PlayerPunishment>(sql, (p, p1, p2) => 
                {
                    p.Player = p1;
                    p.Punisher = p2;
                    return p;
                }, splitOn:"PlayerId,PlayerId").ToList();
            }
        }

        public static List<PlayerPunishment> GetPlayerPunishments(this IDatabaseManager database, string playerId, bool punisher = false)
        {
            string column = punisher ? "p.PunisherId" : "p.PlayerId";
            string sql = "SELECT p.*, p1.*, p2.* FROM dbo.PlayerPunishments p JOIN dbo.Players p1 ON p.PlayerId = p1.PlayerId " +
                $"JOIN dbo.Players p2 ON p.PunisherId = p2.PlayerId WHERE {column} = @playerId;";

            using (var conn = database.Connection)
            {
                return conn.Query<PlayerPunishment, Player, Player, PlayerPunishment>(sql, (p, p1, p2) =>
                {
                    p.Player = p1;
                    p.Punisher = p2;
                    return p;
                }, new { playerId }, splitOn: "PlayerId,PlayerId").ToList();
            }
        }

        public static PlayerPunishment GetPunishment(this IDatabaseManager database, int punishmentId)
        {
            string sql = "SELECT p.*, p1.*, p2.* FROM dbo.PlayerPunishments p JOIN dbo.Players p1 ON p.PlayerId = p1.PlayerId " +
                "JOIN dbo.Players p2 ON p.PunisherId = p2.PlayerId WHERE p.PunishmentId = @punishmentId;";

            using (var conn = database.Connection)
            {
                return conn.Query<PlayerPunishment, Player, Player, PlayerPunishment>(sql, (p, p1, p2) =>
                {
                    p.Player = p1;
                    p.Punisher = p2;
                    return p;
                }, new { punishmentId }, splitOn: "PlayerId,PlayerId").FirstOrDefault();
            }
        }

        public static List<PlayerPunishment> GetActiveBans(this IDatabaseManager database)
        {
            string sql = "SELECT * FROM dbo.PlayerPunishments WHERE ExpiryDate > SYSDATETIME();";

            using (var conn = database.Connection)
            {
                return conn.Query<PlayerPunishment>(sql).ToList();
            }
        }
    }
}
