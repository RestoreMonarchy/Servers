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
using System.Threading.Tasks;

namespace Web.Server.Controllers
{
    [ApiController]
    [Authorize(Roles = "Admin,Moderator")]
    [Route("api/[controller]")]
    public class TicketsController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly HttpClient httpClient;

        private SqlConnection connection => new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

        public TicketsController(IConfiguration configuration, HttpClient httpClient)
        {
            this.configuration = configuration;
            this.httpClient = httpClient;
        }

        [HttpPost]
        public int CreateTicket([FromBody] Ticket ticket)
        {
            string sql = "INSERT INTO dbo.Tickets (TicketTitle, TicketContent, TicketCategory, TicketAuthor, TargetTicketId) " +
                "VALUES (@TicketTitle, @TicketContent, @TicketCategory, @TicketAuthor, @TargetTicketId);";

            ticket.TicketAuthor = User.FindFirst(x => x.Type == ClaimTypes.Name).Value;
            if (ticket.TicketTitle == null || ticket.TicketCategory == null || ticket.TicketContent == null || ticket.TicketAuthor == null)
                return 0;

            int rows;
            using (var conn = connection)
            {
                rows = conn.Execute(sql, ticket);
            }
            return rows;
        }

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

        [HttpGet]
        [Authorize]
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

        //[HttpGet("ticketId")]
        //public Ticket GetTicket(int ticketId)
        //{
        //    string sql = "SELECT * FROM dbo.Tickets";
        //}
    }
}
