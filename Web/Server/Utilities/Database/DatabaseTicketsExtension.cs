using Core.Models;
using Dapper;
using System.Collections.Generic;
using System.Linq;

namespace Web.Server.Utilities.Database
{
    public static class DatabaseTicketsExtension
    {
        public static List<Ticket> GetTickets(this DatabaseManager database)
        {
            string sql = "SELECT t.*, p.*, a.*, p2.* FROM dbo.Tickets t LEFT JOIN dbo.Players p ON t.AuthorId = p.PlayerId " +
                "LEFT JOIN dbo.TicketAnswers a ON t.TicketId = a.TicketId LEFT JOIN dbo.Players p2 ON a.AuthorId = p2.PlayerId;";

            List<Ticket> tickets = new List<Ticket>();
            
            using (var conn = database.connection)
            {
                conn.Query<Ticket, Player, TicketAnswer, Player, Ticket>(sql, (t, p, a, p2) => 
                {
                    var ticket = tickets.FirstOrDefault(x => x.TicketId == t.TicketId);
                    if (ticket == null)
                    {
                        ticket = t;
                        ticket.Author = p;
                        ticket.Answers = new List<TicketAnswer>();
                        tickets.Add(ticket);
                    }
                    if (a != null)
                    {
                        a.Author = p2;
                        ticket.Answers.Add(a);
                    }
                    return t;
                }, splitOn: "PlayerId,AnswerId,PlayerId");
            }

            return tickets;
        }

        public static Ticket GetTicket(this DatabaseManager database, int ticketId)
        {
            string sql = "SELECT t.*, p.*, a.*, p2.* FROM dbo.Tickets t LEFT JOIN dbo.Players p ON t.AuthorId = p.PlayerId " +
                "LEFT JOIN dbo.TicketAnswers a ON t.TicketId = a.TicketId LEFT JOIN dbo.Players p2 ON a.AuthorId = p2.PlayerId WHERE t.TicketId = @ticketId;";

            Ticket ticket = null;
            using (var conn = database.connection)
            {
                conn.Query<Ticket, Player, TicketAnswer, Player, Ticket>(sql, (t, p, a, p2) =>
                {
                    if (ticket == null)
                    {
                        ticket = t;
                        ticket.Author = p;
                        ticket.Answers = new List<TicketAnswer>();
                    }
                    if (a != null)
                    {
                        a.Author = p2;
                        ticket.Answers.Add(a);
                    }
                    return t;
                }, new { ticketId }, splitOn: "PlayerId,AnswerId,PlayerId");
            }

            return ticket;
        }

        public static int CreateTicket(this DatabaseManager database, Ticket ticket)
        {
            string sql = "INSERT INTO dbo.Tickets (Title, Content, Category, AuthorId, LastUpdate, CreateDate) " +
                "OUTPUT INSERTED.TicketId VALUES (@Title, @Content, @Category, @AuthorId, @LastUpdate, @CreateDate);";

            using (var conn = database.connection)
            {
                return conn.ExecuteScalar<int>(sql, ticket);
            }
        }

        public static int CreateAnswer(this DatabaseManager database, TicketAnswer answer)
        {
            string sql = "INSERT INTO dbo.TicketAnswers (TicketId, Content, AuthorId, LastUpdate, CreateDate) OUTPUT INSERTED.AnswerId " +
                "VALUES (@TicketId, @Content, @AuthorId, @LastUpdate, @CreateDate);";

            using (var conn = database.connection)
            {
                return conn.ExecuteScalar<int>(sql, answer);
            }
        }

        public static bool ToggleTicket(this DatabaseManager database, int ticketId)
        {
            string sql = "UPDATE dbo.Tickets SET Status = ~Status OUTPUT INSERTED.Status WHERE TicketId = @ticketId;";
            
            using (var conn = database.connection)
            {
                return conn.ExecuteScalar<bool>(sql, new { ticketId });
            }
        }
    }
}
