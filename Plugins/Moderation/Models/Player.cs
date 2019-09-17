using RestoreMonarchy.Moderation.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestoreMonarchy.Moderation.Models
{
    public class Player
    {
        public Player(ulong playerId, string playerName = null, string ip = null)
        {
            PlayerId = playerId;
            if (ip != null)
                PlayerCountry = PlayerHelper.DownloadCountry(ip);

            if (playerName == null)
                PlayerName = PlayerHelper.DownloadName(playerId);
            else
                PlayerName = playerName;

            PlayerCreated = DateTime.Now;
        }

        public Player() { }

        public ulong PlayerId { get; set; }
        public string PlayerName { get; set; }
        public string PlayerCountry { get; set; }
        public DateTime PlayerCreated { get; set; }

        public virtual List<Ban> Bans { get; set; }
        
    }
}
