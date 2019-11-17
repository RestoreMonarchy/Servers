using AspNet.Security.ApiKey.Providers;
using Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestoreMonarchy.Database;
using System.Collections.Generic;
using Web.Server.Extensions.Database;

namespace Web.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RanksController : ControllerBase
    {
        private readonly IDatabaseManager _database;

        public RanksController(IDatabaseManager database)
        {
            this._database = database;
        }

        [Authorize(Roles = "Moderator, Admin")]
        [HttpGet]
        public List<Rank> GetRanks()
        {
            return _database.GetRanksServer();
        }

        [Authorize(Roles = "Moderator, Admin")]
        [HttpPost]
        public short CreateRank([FromBody] Rank rank)
        {
            return _database.CreateRank(rank);
        }

        [Authorize(Roles = "Moderator, Admin")]
        [HttpPut("{rankId}")]
        public void PutPermission(int rankId, [FromQuery] string permission)
        {
            _database.AddPermission(rankId, permission);
        }

        [Authorize(Roles = "Moderator, Admin")]
        [HttpDelete("{rankId}")]
        public void DeletePermission(int rankId, [FromQuery] string permission)
        {
            _database.DeletePermission(rankId, permission);
        }

        [HttpGet("search")]
        public Dictionary<short, string> GetRanksSearch()
        {
            return _database.GetRanksSearch();
        }

        [HttpGet("{playerId}")]
        public List<PlayerRank> GetPlayerRanks(string playerId)
        {
            return _database.GetPlayerRanks(playerId);
        }

        [HttpGet("server")]
        [Authorize(AuthenticationSchemes = ApiKeyDefaults.AuthenticationScheme)]
        public List<Rank> GetRanksServer()
        {
            return _database.GetRanksServer();
        }
    }
}
