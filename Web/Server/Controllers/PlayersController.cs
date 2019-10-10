using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Core.Models;
using Dapper;
using Web.Server.Utilities;
using Web.Server.Utilities.DiscordMessager;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Security.Claims;

namespace Web.Server.Controllers
{
    [ApiController]
    [Authorize(Roles = "Moderator, Admin")]
    [Route("api/[controller]")]
    public class PlayersController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly Database database;
        private readonly DiscordMessager messager;
        private readonly ImageManager avatarManager;

        private SqlConnection connection => new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

        public PlayersController(IConfiguration configuration, Database database, DiscordMessager messager, ImageManager avatarManager)
        {            
            this.configuration = configuration;
            this.database = database;
            this.messager = messager;
            this.avatarManager = avatarManager;
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
                    avatarManager.SaveSteamPlayerAvatar(player.PlayerId);
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