using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Core.Models
{
    public class Ticket
    {
        public int TicketId { get; set; }
        [Required]
        [StringLength(40, ErrorMessage = "Title is too long.")]
        public string TicketTitle { get; set; }
        [Required]
        [StringLength(5000, ErrorMessage = "Content is too long.")]
        public string TicketContent { get; set; }
        [Required]
        public string TicketCategory { get; set; }
        public string TicketAuthorId { get; set; }
        public int? TargetTicketId { get; set; }
        public DateTime TicketUpdate { get; set; }
        public DateTime TicketCreated { get; set; }

        public virtual Player Author { get; set; }
        public virtual List<Ticket> Responses { get; set; }
    }
}
