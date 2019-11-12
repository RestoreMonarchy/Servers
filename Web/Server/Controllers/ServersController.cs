using Core.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Okolni.Source.Query;

namespace Web.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServersController : ControllerBase
    {
        private readonly string steamDevKey;
        public ServersController(IConfiguration configuration)
        {
            steamDevKey = configuration["SteamAPI"];
        }

        public List<GameServer> Servers = new List<GameServer>()
        {
            new GameServer(1, "EU1", "#1 Restore Monarchy", "restoremonarchy.com", "46.242.131.22", 27015, 27016)
        };

        [HttpGet("info")]
        public List<GameServer> GetServers()
        {            
            List<GameServer> servers = new List<GameServer>();

            foreach (var server in Servers)
            {
                IQueryConnection query = new QueryConnection();
                query.Host = server.IP;
                query.Port = server.QueryPort;
                query.Connect();
                var info = query.GetInfo();
                server.Status = new GameServerStatus(info.Name, info.Players, info.MaxPlayers, info.Map, info.HasPort ? info.Port.Value : default(int));
                servers.Add(server);
            }

            return servers;
        }
    }
}
