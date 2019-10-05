using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Core.Models;
using Dapper;
using Web.Server.Utilities;
using Web.Server.Utilities.DiscordMessager;
using Microsoft.AspNetCore.Authorization;

namespace Web.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlayersController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly Database database;
        private readonly DiscordMessager messager;
        private readonly AvatarManager avatarManager;

        private SqlConnection connection => new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

        public PlayersController(IConfiguration configuration, Database database, DiscordMessager messager, AvatarManager avatarManager)
        {            
            this.configuration = configuration;
            this.database = database;
            this.messager = messager;
            this.avatarManager = avatarManager;
        }

        [HttpPost]
        public ActionResult<int> AddPlayer([FromBody] Player player)
        {
            string sql = "INSERT INTO dbo.Players (PlayerId, PlayerName, PlayerCountry) VALUES (@PlayerId, @PlayerName, @PlayerCountry);";
            int rows = 0;
            using (var conn = connection)
            {
                rows = conn.Execute(sql, new { PlayerId = (long)player.PlayerId, player.PlayerName, player.PlayerCountry });
            }

            if (rows > 0)
            {
                Task.Factory.StartNew(() =>
                {
                    messager.SendPlayerCreatedWebhook(player);
                    avatarManager.SaveSteamPlayerAvatar(player.PlayerId);
                });
            }

            return Ok(rows);
        }

        [HttpGet("{playerId}")]
        public ActionResult<Player> GetPlayer(ulong playerId)
        {
            Player player = database.GetPlayer(playerId);

            if (player != null)
                return Ok(player);
            else
                return NotFound(player);
        }
    }
}