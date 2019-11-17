using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Core.Models
{
    public class Rank
    {
        public short RankId { get; set; }
        [Required]
        public string ShortName { get; set; }
        [Required]
        public string Name { get; set; }
        public string PermissionTags { get; set; }
        [Required]
        public short ValidDays { get; set; }
        public DateTime CreateDate { get; set; }
        public bool ActiveFlag { get; set; }

        public virtual List<string> Members { get; set; }
    }
}
