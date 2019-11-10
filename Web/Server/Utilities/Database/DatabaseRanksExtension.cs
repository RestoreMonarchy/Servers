using Core.Models;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Server.Utilities.Database
{
    public static class DatabaseRanksExtension
    {
        public static List<Rank> GetRanks(this DatabaseManager database)
        {
            string sql = "SELECT * FROM dbo.Ranks;";
            using (var conn = database.connection) 
            {
                return conn.Query<Rank>(sql).ToList();
            }
        }

        public static List<Rank> GetPlayerRanks(this DatabaseManager database)
        {
            string sql = "SELECT r.*, p.PlayerId FROM dbo.Ranks r JOIN dbo.PlayerRanks p ON r.RankId = p.RankId AND p.ValidUntil > SYSDATETIME();";

            List<Rank> ranks = new List<Rank>();
            using (var conn = database.connection)
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

                    rank.Members.Add(p);
                    
                    return rank;
                }, splitOn: "PlayerId");
            }

            return ranks;            
        }

        public static void RankPlayer(this DatabaseManager database, PlayerRank playerRank)
        {
            string sql = "INSERT INTO dbo.PlayerRanks (PlayerId, RankId, ValidUntil) VALUES (@PlayerId, @RankId, @ValidUntil);";

            using (var conn = database.connection)
            {
                conn.Execute(sql, playerRank);
            }
        }

        public static Dictionary<short, string> GetRanksSearch(this DatabaseManager database)
        {
            string sql = "SELECT RankId, Name, ValidDays FROM dbo.Ranks;";

            using (var conn = database.connection)
            {
                return conn.Query(sql).ToDictionary(x => (short)x.RankId, x => $"{x.Name} [{x.ValidDays} days]");
            }
        }
    }
}
