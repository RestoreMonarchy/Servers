using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models
{
    public class Player
    {
        public Player(string id, string name, string country)
        {
            PlayerId = id;
            PlayerName = name;
            PlayerCountry = country;
            PlayerLastActivity = DateTime.Now;
        }

        public Player() { }
        public string PlayerId { get; set; }
        public string PlayerName { get; set; }
        public string PlayerCountry { get; set; }
        public byte[] PlayerAvatar { get; set; }
        public decimal Balance { get; set; }
        public string Role { get; set; }
        public DateTime PlayerLastActivity { get; set; }
        public DateTime PlayerCreated { get; set; }

        public virtual List<PlayerBan> PlayerBans { get; set; }
    }
}
