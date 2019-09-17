using Core.Models;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System;
using System.Timers;

namespace Kits.Models
{
    public class KitCooldown
    {
        public UnturnedPlayer Player { get; set; }
        public Kit Kit { get; set; }
        public Timer Timer { get; set; }
        public DateTime TimeStarted { get; set; }

        public KitCooldown(UnturnedPlayer player, Kit kit)
        {
            Player = player;
            Kit = kit;
            Timer = new Timer(kit.Cooldown * 1000);
            Timer.AutoReset = false;
            Timer.Elapsed += (sender, e) =>
            {
                UnturnedChat.Say(Player, KitsPlugin.Instance.Translate("CooldownExpiredNotify", kit.Name), KitsPlugin.Instance.MessageColor);
            };
        }

        public void Start()
        {
            Timer.Start();
            TimeStarted = DateTime.Now;
        }
    }
}
