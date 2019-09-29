using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Core.Models;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using RestoreMonarchy.WebAPI.Utilities;
using RestoreMonarchy.WebAPI.Models;

namespace WebAPI.Controllers
{
    [ApiController]
    public class PlayerBansController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly DiscordMessager messager;

        private SqlConnection connection => new SqlConnection(configuration["ConnectionString"]);

        public PlayerBansController(IConfiguration configuration, DiscordMessager messager)
        {
            this.configuration = configuration;
            this.messager = messager;
        }

        [HttpPost("api/PlayerBans")]
        public ActionResult<int> AddBan([FromBody] PlayerBan ban)
        {
            string sql = "INSERT INTO dbo.PlayerBans (PlayerId, PunisherId, BanReason, BanDuration) OUTPUT INSERTED.BanId VALUES (@PlayerId, @PunisherId, @BanReason, @BanDuration);";

            int banId;

            using (var conn = connection)
            {
                banId = conn.ExecuteScalar<int>(sql, new { PlayerId = (long)ban.PlayerId, PunisherId = (long)ban.PunisherId, ban.BanReason, ban.BanDuration });
            }

            Task.Factory.StartNew(() =>
            {
                messager.SendBanWebhook(banId);
            });
            
            return banId;
        }
    }
}