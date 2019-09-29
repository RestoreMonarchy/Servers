using Core.Models;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using RestoreMonarchy.WebAPI.Models;
using RestoreMonarchy.WebAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace RestoreMonarchy.WebAPI.Controllers
{
    [ApiController]
    public class PlayersController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly Database database;
        private readonly DiscordMessager messager;

        private SqlConnection connection => new SqlConnection(configuration["ConnectionString"]);

        public PlayersController(IConfiguration configuration, Database database, DiscordMessager messager)
        {
            this.configuration = configuration;
            this.database = database;
            this.messager = messager;
        }

        [HttpPost("api/Players")]
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
                });                              
            }

            return Ok(rows);
        }

        [HttpGet("api/Players/{playerId}")]
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
