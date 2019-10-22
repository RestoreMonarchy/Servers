using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Core.Models
{
    public class PlayerPunishment
    {
        public int PunishmentId { get; set; }
        [Required]
        public string PlayerId { get; set; }
        public string PunisherId { get; set; }
        [Required]
        public string Category { get; set; }
        [Required]
        [StringLength(1000, ErrorMessage = "Content is too long.")]
        public string Reason { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public DateTime CreateDate { get; set; }

        public Player Player { get; set; }
        public Player Punisher { get; set; }
    }
}
