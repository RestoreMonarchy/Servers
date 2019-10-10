using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models
{
    public class PlayerBan
    {
        public PlayerBan(string playerId, string punisherId, string reason = null, int? duration = null)
        {
            PlayerId = playerId;
            PunisherId = punisherId;
            BanReason = reason;
            BanDuration = duration;
        }
        public PlayerBan() { }
        public int BanId { get; set; }
        public string PlayerId { get; set; }
        public string PunisherId { get; set; }
        public string BanReason { get; set; }
        public int? BanDuration { get; set; }
        public DateTime BanCreated { get; set; }
        public bool SendFlag { get; set; }

        public virtual Player Player { get; set; }
        public virtual Player Punisher { get; set; }
    }
}
