using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Server.Utilities.Database;
using Web.Server.Utilities.DiscordMessager;

namespace Web.Server.Controllers
{
    [ApiController]
    [Authorize(Roles = "Admin,Moderator")]
    [Route("api/[controller]")]
    public class PunishmentsController : ControllerBase
    {
        private readonly DatabaseManager _database;
        private readonly DiscordMessager _messager;
        public PunishmentsController(DatabaseManager database, DiscordMessager messager)
        {
            _database = database;
            _messager = messager;
        }

        [HttpPost]
        public async Task<PlayerPunishment> CreatePunishment([FromBody] PlayerPunishment punishment)
        {
            punishment.PunisherId = User.Identity.Name;
            punishment.CreateDate = DateTime.Now;
            punishment.PunishmentId = _database.CreatePunishment(punishment);

            await Task.Factory.StartNew(async () =>
            {
                await _messager.SendPunishmentWebhook(punishment.PunishmentId);
            });

            return punishment;
        }

        [HttpGet("dashboard")]
        public List<PlayerPunishment> GetPunishments()
        {
            return _database.GetPunishments();
        }

        [HttpGet]
        [Authorize]
        public List<PlayerPunishment> GetMyPunishments()
        {
            return _database.GetPlayerPunishments(User.Identity.Name);    
        }
    }
}
