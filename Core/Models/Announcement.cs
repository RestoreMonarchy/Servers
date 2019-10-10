using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models
{
    public class Announcement
    {
        public int AnnouncementId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string AuthorId { get; set; }
        public DateTime UpdateDate { get; set; }
        public DateTime CreateDate { get; set; }

        public virtual Player Author { get; set; }
    }
}
