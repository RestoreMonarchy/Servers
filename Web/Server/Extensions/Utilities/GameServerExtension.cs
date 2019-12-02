using Core.Models;
using Okolni.Source.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Web.Server.Extensions.Utilities
{
    public static class GameServerExtension
    {
        public static void RefreshStatus(this GameServer server)
        {
            IQueryConnection query = new QueryConnection();
            query.Host = server.IP;
            query.Port = server.QueryPort;
            query.Connect();
            var info = query.GetInfo();
            server.Status = new GameServerStatus(info.Name, info.Players, info.MaxPlayers, info.Map, info.HasPort ? info.Port.Value : default(int));
        }
        
        public static void RefreshStatus(this IEnumerable<GameServer> servers)
        {
            foreach (var server in servers)
            {
                server.RefreshStatus();
            }
        }
    }
}
