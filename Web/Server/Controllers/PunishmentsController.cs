using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestoreMonarchy.Database;
using Web.Server.Extensions.Database;
using Web.Server.Utilities.DiscordMessager;

namespace Web.Server.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class PunishmentsController : ControllerBase
    {
        private readonly IDatabaseManager _database;
        private readonly DiscordMessager _messager;
        public PunishmentsController(IDatabaseManager database, DiscordMessager messager)
        {
            _database = database;
            _messager = messager;
        }

        [Authorize(Roles = "Admin,Moderator")]
        [HttpPost]
        public async Task<PlayerPunishment> CreatePunishment([FromBody] PlayerPunishment punishment)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            
            punishment.PunisherId = User.FindFirst(ClaimTypes.Name).Value;
            punishment.CreateDate = DateTime.Now;
            punishment.PunishmentId = _database.CreatePunishment(punishment);

            punishment = _database.GetPunishment(punishment.PunishmentId);

            await Task.Factory.StartNew(async () =>
            {
                await _messager.SendPunishmentWebhook(punishment.PunishmentId);
            });

            return punishment;
        }

        [Authorize(Roles = "Admin,Moderator")]
        [HttpGet("dashboard")]
        public List<PlayerPunishment> GetPunishments()
        {
            return _database.GetPunishments();
        }

        [HttpGet]
        public List<PlayerPunishment> GetMyPunishments()
        {
            var punishments = _database.GetPlayerPunishments(User.FindFirst(ClaimTypes.Name).Value);

            return punishments;    
        }
    }
}
