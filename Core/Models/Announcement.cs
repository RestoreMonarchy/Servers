using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Core.Models
{
    public class Announcement
    {
        public int AnnouncementId { get; set; }
        [Required]
        [StringLength(40, ErrorMessage = "Title is too long.")]
        public string Title { get; set; }
        [Required]
        [StringLength(6000, ErrorMessage = "Content is too long.")]
        public string Content { get; set; }
        public string AuthorId { get; set; }
        public DateTime UpdateDate { get; set; }
        public DateTime CreateDate { get; set; }

        public virtual Player Author { get; set; }
    }
}
