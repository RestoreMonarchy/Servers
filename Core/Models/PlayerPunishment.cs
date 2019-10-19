using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models
{
    public class PlayerPunishment
    {
        public int PunishmentId { get; set; }
        public string PlayerId { get; set; }
        public string PunisherId { get; set; }
        public string Category { get; set; }
        public string Reason { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public DateTime CreateDate { get; set; }

        public Player Player { get; set; }
        public Player Punisher { get; set; }
    }
}
