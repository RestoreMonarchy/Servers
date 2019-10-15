using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Core.Models
{
    public class TicketAnswer
    {
        public TicketAnswer() { }

        public int AnswerId { get; set; }
        public int TicketId { get; set; }
        [Required]
        [StringLength(4000, ErrorMessage = "Content is too long.")]
        public string Content { get; set; }
        public string AuthorId { get; set; }
        public DateTime LastUpdate { get; set; }
        public DateTime CreateDate { get; set; }

        public virtual Player Author { get; set; }
        public virtual Ticket Ticket { get; set; }
    }
}
