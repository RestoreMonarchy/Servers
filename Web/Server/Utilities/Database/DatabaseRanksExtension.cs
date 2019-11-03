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

        public static Dictionary<short, string> GetRanksSearch(this DatabaseManager database)
        {
            string sql = "SELECT RankId, Name, ValidDays FROM dbo.Ranks;";

            using (var conn = database.connection)
            {
                return conn.Query(sql).ToDictionary(x => (short)x.RankId, x => $"{x.PlayerName} ({x.ValidDays})");
            }
        }
    }
}
