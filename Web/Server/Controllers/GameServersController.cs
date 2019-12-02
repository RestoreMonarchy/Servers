using Core.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Web.Server.Extensions.Utilities;
using RestoreMonarchy.Database;
using Web.Server.Extensions.Database;
using Microsoft.AspNetCore.Authorization;

namespace Web.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GameServersController : ControllerBase
    {
        private readonly IDatabaseManager _database;
        private List<GameServer> GameServers { get; set; }
        public GameServersController(IDatabaseManager database)
        {
            _database = database;
            GameServers = _database.GetGameServers();
        }

        [HttpGet]
        public List<GameServer> GetServers([FromQuery] bool updateStatus = true)
        {
            if (updateStatus || GameServers.Exists(x => x.Status == null))
            {
                GameServers.RefreshStatus();
            }

            return GameServers;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("refresh")]
        public void RefreshServers()
        {
            GameServers = _database.GetGameServers();
        }
    }
}
