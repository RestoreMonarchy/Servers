using Core.Models;
using Kits.Models;
using Rocket.API;
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
        private KitsPlugin pluginInstance => KitsPlugin.Instance;
        public void Execute(IRocketPlayer caller, string[] command)
        {   
            var player = (UnturnedPlayer)caller;

            if (command.Length < 1)
            {
                UnturnedChat.Say(caller, pluginInstance.Translate("KitFormat"), pluginInstance.MessageColor);
                return;
            }

            Kit kit = pluginInstance.KitsCache.FirstOrDefault(x => x.Name.Equals(command[0], StringComparison.OrdinalIgnoreCase));
            if (kit == null)
            {
                UnturnedChat.Say(caller, pluginInstance.Translate("KitNotFound"), pluginInstance.MessageColor);
                return;
            }

            if (!caller.HasPermission("kit." + kit.Name))
            {
                UnturnedChat.Say(caller, pluginInstance.Translate("KitNoPermission"), pluginInstance.MessageColor);
                return;
            }

            KitCooldown cooldown = pluginInstance.Cooldowns.FirstOrDefault(x => x.Player.CSteamID.Equals(player.CSteamID) && x.Kit.Name.Equals(kit.Name));

            if (cooldown != null && cooldown.Timer.Enabled)
            {
                UnturnedChat.Say(caller, pluginInstance.Translate("KitCooldown", (kit.Cooldown - (DateTime.Now - cooldown.TimeStarted).TotalSeconds).ToString("0")),
                    pluginInstance.MessageColor);
                return;
            }

            foreach (var item in kit.Items)
            {
                Item itemObj = new Item(item.ItemId, true);
                itemObj.metadata = item.State;
                player.Player.inventory.forceAddItem(itemObj, true);
            }

            if (kit.Experience > 0)
            {
                player.Experience += kit.Experience;
            }

            if (kit.Vehicle != 0)
            {
                player.GiveVehicle(kit.Vehicle);
            }

            if (cooldown == null)
            {
                cooldown = new KitCooldown(player, kit);
                pluginInstance.Cooldowns.Add(cooldown);
            }

            cooldown.Start();
            UnturnedChat.Say(caller, pluginInstance.Translate("KitSuccess", kit.Name), pluginInstance.MessageColor);
        }

        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "kit";

        public string Help => "Gives you a kit";

        public string Syntax => "<name>";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string>();
    }
}
