using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models
{
    public class Rank
    {
        public short RankId { get; set; }
        public string ShortName { get; set; }
        public string Name { get; set; }
        public string PermissionTags { get; set; }
        public short ValidDays { get; set; }
        public DateTime CreateDate { get; set; }
        public bool ActiveFlag { get; set; }

        public virtual List<string> Members { get; set; }
    }
}
