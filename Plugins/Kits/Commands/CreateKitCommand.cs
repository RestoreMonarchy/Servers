using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using System.Linq;
using Core.Models;
using SDG.Unturned;
using System;

namespace Kits.Commands
{
    public class CreateKitCommand : IRocketCommand
    {
        private KitsPlugin pluginInstance => KitsPlugin.Instance;
        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;

            if (command.Length < 2)
            {
                UnturnedChat.Say(caller, pluginInstance.Translate("CreateKitFormat"), pluginInstance.MessageColor);
                return;
            }

            Kit kit = new Kit();

            kit.Name = command[0];
            if (pluginInstance.KitsCache.Exists(x => x.Name.Equals(kit.Name, StringComparison.OrdinalIgnoreCase)))
            {
                UnturnedChat.Say(caller, pluginInstance.Translate("CreateKitExists"), pluginInstance.MessageColor);
                return;
            }

            int cooldown = 0;
            if (!int.TryParse(command[1], out cooldown))
            {
                UnturnedChat.Say(caller, pluginInstance.Translate("CreateKitInvalidCooldown"), pluginInstance.MessageColor);
                return;
            }

            kit.Cooldown = cooldown;
            uint experience = 0;
            uint.TryParse(command.ElementAtOrDefault(0), out experience);
            kit.Experience = experience;

            kit.Items = new List<Kit.Item>();

            var clothing = player.Player.clothing;

            if (clothing.backpack != 0)
                kit.Items.Add(new Kit.Item(clothing.backpack, clothing.backpackState));
            if (clothing.vest != 0)
                kit.Items.Add(new Kit.Item(clothing.vest, clothing.vestState));
            if (clothing.shirt != 0)
                kit.Items.Add(new Kit.Item(clothing.shirt, clothing.shirtState));
            if (clothing.pants != 0)
                kit.Items.Add(new Kit.Item(clothing.pants, clothing.pantsState));
            if (clothing.mask != 0)
                kit.Items.Add(new Kit.Item(clothing.mask, clothing.maskState));
            if (clothing.hat != 0)
                kit.Items.Add(new Kit.Item(clothing.hat, clothing.hatState));
            if (clothing.glasses != 0)
                kit.Items.Add(new Kit.Item(clothing.glasses, clothing.glassesState));

            for (byte num = 0; num < PlayerInventory.PAGES - 2; num++)
            {
                for (byte num2 = 0; num2 < player.Inventory.getItemCount(num); num2++)
                {
                    var item = player.Inventory.getItem(num, num2);
                    if (item == null)
                        continue;

                    kit.Items.Add(new Kit.Item(item.item.id, item.item.metadata));
                }
            }

            pluginInstance.PostKit(kit);

            UnturnedChat.Say(caller, pluginInstance.Translate("CreateKitSuccess", kit.Name, kit.Cooldown, kit.Items.Count), pluginInstance.MessageColor);
        }

        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "createkit";

        public string Help => "Creates a new kit of your inventory";

        public string Syntax => "<name> <cooldown> [experience]";

        public List<string> Aliases => new List<string>()
        {
            "ckit"
        };

        public List<string> Permissions => new List<string>();
    }
}
