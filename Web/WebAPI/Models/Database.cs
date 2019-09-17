using Core.Models;
using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace RestoreMonarchy.WebAPI.Models
{
    public class Database
    {
        private readonly IConfiguration configuration;

        private SqlConnection connection => new SqlConnection(configuration["ConnectionString"]);

        public Database(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public PlayerBan GetPlayerBan(int banId)
        {
            string sql = "SELECT b.*, p.*, p2.* FROM dbo.PlayerBans AS b LEFT JOIN dbo.Players AS p ON p.PlayerId = b.PlayerId " +
                "LEFT JOIN dbo.Players AS p2 ON b.PunisherId = p2.PlayerId WHERE b.BanId = @banId;";

            using (var conn = connection)
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

        public List<PlayerBan> GetActivePlayerBans()
        {

        }
    }
}
