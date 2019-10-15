using Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Web.Server.Utilities.Database;

namespace Web.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TicketsController : Controller
    {
        private readonly DatabaseManager database;

        public TicketsController(DatabaseManager database)
        {
            this.database = database;
        }

        [Authorize]
        [HttpPost("Answer")]
        public TicketAnswer CreateAnswer([FromBody] TicketAnswer answer)
        {
            answer.AuthorId = User.FindFirst(x => x.Type == ClaimTypes.Name).Value;
            answer.LastUpdate = DateTime.Now;
            answer.CreateDate = DateTime.Now;

            // TODO: For some reason when used in TicketPage.razor throw null not implemented exception when adding to list
            // Solved, it wasn't it, it was just because it doesn't return the Author property
            
            answer.AnswerId = database.CreateAnswer(answer);
            return answer;
        }

        [Authorize(Roles = "Admin,Moderator")]
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

        // TODO: Controller for getting a master ticket and it's responses

        [Authorize]
        [HttpGet("{ticketId}")]
        public Ticket GetTicket(int ticketId)
        {
            var ticket = database.GetTicket(ticketId);

            string playerId = User.FindFirst(x => x.Type == ClaimTypes.Name).Value;
            if (!(User.IsInRole("Admin") || User.IsInRole("Moderator") || ticket.AuthorId == playerId || ticket.Answers.Any(x => x.AuthorId == playerId)))
            {
                return null;
            }
            else
            {
                return ticket;
            }
        }
    }
}
