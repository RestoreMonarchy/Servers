using Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestoreMonarchy.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Web.Server.Extensions.Database;
using Web.Server.Utilities;

namespace Web.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TicketsController : Controller
    {
        private readonly IDatabaseManager database;
        public TicketsController(IDatabaseManager database)
        {
            this.database = database;
        }

        [Authorize]
        [HttpPost("Answer")]
        [Cooldown(Cooldown = 60)]
        public TicketAnswer CreateAnswer([FromBody] TicketAnswer answer)
        {
            var steamId = User.FindFirst(x => x.Type == ClaimTypes.Name).Value;
            if (!(User.IsInRole("Moderator") || User.IsInRole("Admin")))
            {
                var ticket = database.GetTicket(answer.TicketId);
                if (!(ticket.AuthorId == steamId || ticket.Answers.Exists(x => x.AuthorId == steamId)))
                    return null;
            }

            answer.AuthorId = steamId;
            answer.LastUpdate = DateTime.Now;
            answer.CreateDate = DateTime.Now;
            
            answer.AnswerId = database.CreateAnswer(answer);
            answer.Author = database.GetPlayer(steamId);
            return answer;
        }

        [Authorize]
        [HttpPost]
        public Ticket CreateTicket([FromBody] Ticket ticket)
        {            
            ticket.AuthorId = User.FindFirst(x => x.Type == ClaimTypes.Name).Value;
            ticket.LastUpdate = DateTime.Now;
            ticket.CreateDate = DateTime.Now;

            if (ticket == null || ticket.Title == null || ticket.Category == null || ticket.Content == null || ticket.AuthorId == null)
                return null;

            ticket.TicketId = database.CreateTicket(ticket);
            return ticket;
        }

        [Authorize(Roles = "Admin,Moderator")]
        [HttpGet("dashboard")]
        public List<Ticket> GetTickets()
        {            
            return database.GetTickets();
        }

        [Authorize]
        [HttpGet]
        public List<Ticket> GetMyTickets()
        {
            string playerId = User.FindFirst(x => x.Type == ClaimTypes.Name).Value;
            var tickets = database.GetTickets();
            return tickets.Where(x => x.AuthorId == playerId || x.Answers.Exists(y => y.AuthorId == playerId)).ToList();
        }

        [Authorize]
        [HttpGet("{ticketId}")]
        public ActionResult<Ticket> GetTicket(int ticketId)
        {
            var ticket = database.GetTicket(ticketId);
            string playerId = User.FindFirst(x => x.Type == ClaimTypes.Name).Value;
            if (!(User.IsInRole("Admin") || User.IsInRole("Moderator") || ticket.AuthorId == playerId || ticket.Answers.Any(x => x.AuthorId == playerId)))
            {
                return Unauthorized();
            }
            else
            {
                return ticket;
            }
        }

        [Authorize(Roles = "Admin,Moderator")]
        [HttpPut("{ticketId}")]
        public bool ToggleTicket(int ticketId)
        {
            return database.ToggleTicket(ticketId);
        }
    }
}
