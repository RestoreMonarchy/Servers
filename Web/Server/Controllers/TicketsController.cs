using Core.Models;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;

namespace Web.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TicketsController : Controller
    {
        private readonly IConfiguration configuration;

        private SqlConnection connection => new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

        public TicketsController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        [Authorize(Roles = "Admin,Moderator")]
        [HttpPost]
        public Ticket CreateTicket([FromBody] Ticket ticket)
        {
            string sql = "INSERT INTO dbo.Tickets (TicketTitle, TicketContent, TicketCategory, TicketAuthorId, TargetTicketId, TicketUpdate, TicketCreated) " +
                "OUTPUT INSERTED.TicketId VALUES(@TicketTitle, @TicketContent, @TicketCategory, @TicketAuthorId, @TargetTicketId, @TicketUpdate, @TicketCreated);";

            ticket.TicketAuthorId = User.FindFirst(x => x.Type == ClaimTypes.Name).Value;
            ticket.TicketUpdate = DateTime.Now;
            ticket.TicketCreated = DateTime.Now;

            if (ticket == null || ticket.TicketTitle == null || ticket.TicketCategory == null || ticket.TicketContent == null || ticket.TicketAuthorId == null)
                return null;

            using (var conn = connection)
            {
                ticket.TicketId = conn.ExecuteScalar<int>(sql, ticket);
            }
            return ticket;
        }

        [Authorize(Roles = "Admin,Moderator")]
        [HttpGet("dashboard")]
        public List<Ticket> GetTickets()
        {
            string sql = "SELECT * FROM dbo.Tickets;";

            List<Ticket> tickets = new List<Ticket>();
            using (var conn = connection)
            {
                tickets = conn.Query<Ticket>(sql).ToList();
            }

            foreach (var ticket in tickets)
            {                
                if (ticket.TargetTicketId.HasValue)
                {
                    var target = tickets.FirstOrDefault(x => x.TicketId == ticket.TargetTicketId.Value);

                    target.Responses.Add(ticket);
                    tickets.Remove(ticket);
                } else
                {
                    ticket.Responses = new List<Ticket>();
                }
            }

            return tickets;
        }

        [Authorize]
        [HttpGet]
        public List<Ticket> GetMyTickets()
        {
            string sql = "SELECT * FROM dbo.GetMyTickets(@playerId);";
            string playerId = User.FindFirst(x => x.Type == ClaimTypes.Name).Value;

            List<Ticket> tickets = new List<Ticket>();

            using (var conn = connection)
            {
                conn.Query<Ticket, Player, Ticket>(sql, (t, p) =>
                {
                    t.Author = p;
                    if (t.TargetTicketId.HasValue)
                    {
                        var main = tickets.First(x => x.TicketId == t.TargetTicketId.Value);
                        main.Responses.Add(t);
                    }
                    else
                    {
                        t.Responses = new List<Ticket>();
                        tickets.Add(t);
                    }                    
                    return t;
                }, new { playerId }, splitOn: "PlayerName");
            }

            return tickets;
        }

        // TODO: Controller for getting a master ticket and it's responses

        //[Authorize]
        //[HttpGet("{ticketId}")]        
        //public Ticket GetTicket(int ticketId)
        //{
        //    string sql = "SELECT t.*, p.* FROM dbo.Tickets t JOIN dbo.Players p ON t.TicketAuthorId = p.PlayerId " +
        //        "WHERE (t.TicketId = @ticketId OR t.TargetTicketId = @ticketId);" 
        //    if (User.IsInRole("Admin") || User.IsInRole("Moderator"))

        //}
    }
}
