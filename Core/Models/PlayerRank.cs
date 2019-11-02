using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models
{
    public class PlayerRank
    {
        public string PlayerId { get; set; }
        public short RankId { get; set; }
        public DateTime ValidUntil { get; set; }

        public virtual Player Player { get; set; }
        public virtual Rank Rank { get; set; }
    }
}
