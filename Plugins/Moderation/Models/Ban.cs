using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestoreMonarchy.Moderation.Models
{
    public class Ban
    {
        public int BanId { get; set; }
        public ulong PlayerId { get; set; }
        public ulong PunisherId { get; set; }
        public string BanReason { get; set; }
        public int? BanDuration { get; set; }
        public DateTime BanCreated { get; set; }
        public bool SendFlag { get; set; }

        public virtual Player Player { get; set; }
        public virtual Player Punisher { get; set; }
    }
}
