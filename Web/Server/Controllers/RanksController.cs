using AspNet.Security.ApiKey.Providers;
using Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Web.Server.Utilities.Database;

namespace Web.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RanksController : ControllerBase
    {
        private readonly DatabaseManager _database;

        public RanksController(DatabaseManager database)
        {
            this._database = database;
        }

        [HttpGet]
        public List<Rank> GetRanks()
        {
            return _database.GetRanks();
        }

        [HttpGet("search")]
        public Dictionary<short, string> GetRanksSearch()
        {
            return _database.GetRanksSearch();
        }

        [HttpGet("server")]
        [Authorize(AuthenticationSchemes = ApiKeyDefaults.AuthenticationScheme)]
        public List<Rank> GetRanksServer()
        {
            return _database.GetPlayerRanks();
        }
    }
}
