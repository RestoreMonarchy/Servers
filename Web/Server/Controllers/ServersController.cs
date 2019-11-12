using Core.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SteamWebAPI2;
using Microsoft.Extensions.Configuration;
using SteamWebAPI2.Utilities;
using SteamWebAPI2.Interfaces;
using Okolni.Source.Query;
using Okolni.Source.Query.Responses;

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
            new GameServer(1, "EU1", "#1 Restore Monarchy", "restoremonarchy.com", "46.242.131.22", 27015, 27016),
            new GameServer(2, "EU2", "#2 Restore Monarchy", "restoremonarchy.com", "46.242.131.22", 27040, 27041),
            new GameServer(3, "EU3", "#3 Restore Monarchy", "restoremonarchy.com", "45.58.61.82", 27500, 27501),
            new GameServer(4, "EU4", "#4 Restore Monarchy", "restoremonarchy.com", "188.165.235.147", 27018, 27019)
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
