using Core.Models;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
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

        private SqlConnection connection => new SqlConnection(configuration["ConnectionString"]);

        public PlayersController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        [HttpPost("api/Players")]
        public ActionResult<int> AddPlayer([FromBody] Player player)
        {
            string sql = "INSERT INTO dbo.Players (PlayerId, PlayerName, PlayerCountry) VALUES (@PlayerId, @PlayerName, @PlayerCountry);";

            using (var conn = connection)
            {
                return Ok(conn.Execute(sql, new { player.PlayerId, player.PlayerName, player.PlayerCountry }));
            }
        }

        [HttpGet("api/Players/{playerId}")]
        public ActionResult<Player> GetPlayer(ulong playerId)
        {
            string sql = "SELECT p.*, b.* FROM Players AS p LEFT JOIN Bans AS b ON b.PlayerId = p.PlayerId WHERE p.PlayerId = @PlayerId;";
            Player player = null;

            using (var conn = connection)
            {
                conn.Query<Player, PlayerBan, Player>(sql,
                    (p, b) =>
                    {
                        if (player == null)
                            player = p;
                        if (player.PlayerBans == null)
                            player.PlayerBans = new List<PlayerBan>();

                        player.PlayerBans.Add(b);
                        return p;
                    }, new { PlayerId = playerId }, splitOn: "BanId");
            }

            return Ok(player);
        }
    }
}
