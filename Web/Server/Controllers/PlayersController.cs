using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Core.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Web.Server.Extensions.Database;
using System.Collections.Generic;
using AspNet.Security.ApiKey.Providers;
using RestoreMonarchy.Database;
using Web.Server.Services;

namespace Web.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlayersController : ControllerBase
    {
        private readonly IDatabaseManager _database;
        private readonly PlayersService _playersService;

        public PlayersController(IDatabaseManager database, PlayersService playersService)
        {
            _database = database;
            _playersService = playersService;
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult<Player> GetMyPlayer()
        {
            Player player;
            if (User.Identity.IsAuthenticated)
            {
                player = _database.GetPlayer(User.FindFirst(ClaimTypes.NameIdentifier).Value.Substring(37));
            } else
                player = null;

            return player;
        }

        [HttpGet("avatar/{playerId}")]
        [AllowAnonymous]
        public ActionResult GetPlayerAvatar(string playerId)
        {
            return File(_database.GetPlayer(playerId).PlayerAvatar, "image/jpg");
        }

        [HttpGet("search")]
        [AllowAnonymous]
        public Dictionary<string, string> GetPlayersSearch()
        {
            return _database.GetPlayersSearch();
        }

        [HttpGet("{playerId}")]
        [Authorize(AuthenticationSchemes = ApiKeyDefaults.AuthenticationScheme)]
        public async Task<Player> GetPlayer(string playerId, [FromQuery] string ip)
        {
            Player player = await _playersService.GetInitializedPlayerAsync(playerId, ip);
            _database.UpdateLastActivity(playerId);
            return player;
        }
    }
}