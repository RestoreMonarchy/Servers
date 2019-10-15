using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Core.Models
{
    public class Ticket
    {        
        public Ticket() { }
        public Ticket(string title, string content, string category)
        {
            Title = title;
            Content = content;
            Category = category;
        }

        public int TicketId { get; set; }
        [Required]
        [StringLength(60, ErrorMessage = "Title is too long.")]
        public string Title { get; set; }
        [Required]
        [StringLength(4000, ErrorMessage = "Content is too long.")]
        public string Content { get; set; }
        [Required]
        public string Category { get; set; }
        public string AuthorId { get; set; }
        public bool Status { get; set; }
        public DateTime LastUpdate { get; set; }
        public DateTime CreateDate { get; set; }

        public virtual Player Author { get; set; }
        public virtual List<TicketAnswer> Answers { get; set; }
    }
}
