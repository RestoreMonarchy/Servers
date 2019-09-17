using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models
{
    public class PlayerBan
    {
        public PlayerBan(int id, ulong playerId, ulong punisherId, string reason = null, int? duration = null)
        {
            BanId = id;
            PlayerId = playerId;
            PunisherId = punisherId;
            BanReason = reason;
            BanDuration = duration;
        }
        public PlayerBan() { }
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
