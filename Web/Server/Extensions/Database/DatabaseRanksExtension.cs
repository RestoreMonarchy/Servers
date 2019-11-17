using Core.Models;
using Dapper;
using RestoreMonarchy.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Server.Extensions.Database
{
    public static class DatabaseRanksExtension
    {
        public static List<Rank> GetRanks(this IDatabaseManager database)
        {
            string sql = "SELECT * FROM dbo.Ranks;";
            using (var conn = database.Connection) 
            {
                return conn.Query<Rank>(sql).ToList();
            }
        }

        public static short CreateRank(this IDatabaseManager database, Rank rank)
        {
            string sql = "INSERT INTO dbo.Ranks (ShortName, Name, ValidDays) OUTPUT INSERTED.RankId VALUES (@ShortName, @Name, @ValidDays);";

            using (var conn = database.Connection)
            {
                return conn.ExecuteScalar<short>(sql, rank);
            }
        }

        public static List<Rank> GetRanksServer(this IDatabaseManager database)
        {
            string sql = "SELECT r.*, p.PlayerId FROM dbo.Ranks r LEFT JOIN dbo.PlayerRanks p ON r.RankId = p.RankId AND p.ValidUntil > SYSDATETIME();";

            List<Rank> ranks = new List<Rank>();
            using (var conn = database.Connection)
            {
                conn.Query<Rank, string, Rank>(sql, (r, p) =>
                {
                    var rank = ranks.FirstOrDefault(x => x.RankId == r.RankId);
                    if (rank == null)
                    {
                        rank = r;
                        rank.Members = new List<string>();
                        ranks.Add(rank);
                    }

                    if (p != null)
                        rank.Members.Add(p);
                    
                    return rank;
                }, splitOn: "PlayerId");
            }

            return ranks;            
        }

        public static void AddPermission(this IDatabaseManager database, int rankId, string permission)
        {
            string sql = "UPDATE dbo.Ranks SET PermissionTags = CAST(CASE WHEN PermissionTags IS NULL THEN @permission ELSE PermissionTags + ',' + @permission END AS VARCHAR(1024)) WHERE RankId = @rankId;";

            using (var conn = database.Connection)
            {
                conn.Execute(sql, new { rankId, permission });
            }
        }

        public static void DeletePermission(this IDatabaseManager database, int rankId, string permission)
        {
            string sql = "UPDATE dbo.Ranks SET PermissionTags = CAST(CASE WHEN PermissionTags = @permission THEN NULL ELSE REPLACE(PermissionTags, ',' + @permission, '')  END AS VARCHAR(1024)) WHERE RankId = @rankId;";

            using (var conn = database.Connection)
            {
                conn.Execute(sql, new { rankId, permission });
            }
        }

        public static void RankPlayer(this IDatabaseManager database, PlayerRank playerRank)
        {
            string sql = "INSERT INTO dbo.PlayerRanks (PlayerId, RankId, ValidUntil) VALUES (@PlayerId, @RankId, @ValidUntil);";

            using (var conn = database.Connection)
            {
                conn.Execute(sql, playerRank);
            }
        }

        public static List<PlayerRank> GetPlayerRanks(this IDatabaseManager database, string playerId)
        {
            string sql = "SELECT p.*, r.* FROM dbo.PlayerRanks p JOIN dbo.Ranks r ON r.RankId = p.RankId WHERE p.PlayerId = @playerId;";
            
            using (var conn = database.Connection)
            {
                return conn.Query<PlayerRank, Rank, PlayerRank>(sql, (p, r) => 
                {
                    p.Rank = r;
                    return p;
                }, new { playerId }, splitOn: "RankId").ToList();
            }
        }

        public static Dictionary<short, string> GetRanksSearch(this IDatabaseManager database)
        {
            string sql = "SELECT RankId, Name, ValidDays FROM dbo.Ranks;";

            using (var conn = database.Connection)
            {
                return conn.Query(sql).ToDictionary(x => (short)x.RankId, x => $"{x.Name} [{x.ValidDays} days]");
            }
        }
    }
}