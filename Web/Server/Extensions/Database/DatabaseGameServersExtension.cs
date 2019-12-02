using Core.Models;
using Dapper;
using RestoreMonarchy.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Server.Extensions.Database
{
    public static class DatabaseGameServersExtension
    {
        public static List<GameServer> GetGameServers(this IDatabaseManager database)
        {
            string sql = "SELECT * FROM dbo.GameServers;";

            using (var conn = database.Connection)
            {
                return conn.Query<GameServer>(sql).ToList();
            }
        }
    }
}
