using Kits.Models;
using Rocket.API;
using Rocket.API.Collections;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kits.Commands
{
    public class KitCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "kit";

        public string Help => "Gives you a kit";

        public string Syntax => "<name>";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string>();

        public void Execute(IRocketPlayer caller, string[] command)
        {
            var plugin = KitsPlugin.Instance;
            var player = (UnturnedPlayer)caller;

            if (command.Length < 1)
            {
                UnturnedChat.Say(caller, plugin.Translate("KitHelp"), plugin.MessageColor);
                return;
            }

            var kit = plugin.Configuration.Instance.Kits.FirstOrDefault(x => x.Name == command[0]);
            if (kit == null)
            {
                UnturnedChat.Say(caller, plugin.Translate("KitNotFound"), plugin.MessageColor);
                return;
            }

            KitCooldown cooldown = plugin.Cooldowns.FirstOrDefault(x => x.Player.CSteamID == player.CSteamID && x.Kit.Name == kit.Name);

            if (cooldown != null && cooldown.Timer.Enabled)
            {
                UnturnedChat.Say(caller, plugin.Translate("KitCooldown", (kit.Cooldown - (DateTime.Now - cooldown.TimeStarted).TotalSeconds).ToString("0")), 
                    plugin.MessageColor);
                return;
            }

            foreach (var item in kit.MetadataItems)
            {
                if (!player.Inventory.tryAddItem(new Item(item.ItemId, 0, 100, item.Metadata), true))
                    player.GiveItem(new Item(item.ItemId, 0, 100, item.Metadata));
            }
            foreach (var item in kit.Items)
            {
                if (!player.Inventory.tryAddItem(new Item(item, true), true))
                    player.GiveItem(item, 1);
            }

            if (cooldown == null)
            {
                cooldown = new KitCooldown(player, kit);
                plugin.Cooldowns.Add(cooldown);
            }

            cooldown.Start();
            UnturnedChat.Say(caller, plugin.Translate("KitSuccess", kit.Name), plugin.MessageColor);
        }
    }
}
