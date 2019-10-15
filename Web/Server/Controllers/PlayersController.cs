using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Core.Models;
using Web.Server.Utilities;
using Web.Server.Utilities.DiscordMessager;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Web.Server.Utilities.Database;

namespace Web.Server.Controllers
{
    [ApiController]
    [Authorize(Roles = "Moderator, Admin")]
    [Route("api/[controller]")]
    public class PlayersController : ControllerBase
    {
        private readonly DatabaseManager database;
        private readonly DiscordMessager messager;

        public PlayersController(DatabaseManager database, DiscordMessager messager)
        {
            this.database = database;
            this.messager = messager;
        }

        [HttpPost]
        public ActionResult<Player> AddPlayer([FromBody] Player player)
        {
            player = database.CreatePlayer(player);

            if (player != null)
            {
                Task.Factory.StartNew(() =>
                {
                    messager.SendPlayerCreatedWebhook(player);
                });
            }

            return Ok(player);
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult<Player> GetMyPlayer()
        {
            Player player;
            if (User.Identity.IsAuthenticated)
            {
                player = database.GetPlayer(User.FindFirst(ClaimTypes.NameIdentifier).Value.Substring(37));
            } else
                player = null;

            return player;
        }

        [HttpGet("{playerId}")]
        public ActionResult<Player> GetPlayer(string playerId)
        {
            Player player = database.GetPlayer(playerId);

            if (player != null)
                return Ok(player);
            else
                return NotFound(player);
        }
    }
}