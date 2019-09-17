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
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "createkit";

        public string Help => "Creates a new kit of your inventory";

        public string Syntax => "<name> <cooldown>";

        public List<string> Aliases => new List<string>()
        {
            "ckit"
        };

        public List<string> Permissions => new List<string>();

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;
            var plugin = KitsPlugin.Instance;

            if (command.Length < 2)
            {
                UnturnedChat.Say(caller, plugin.Translate("CreateKitHelp"), plugin.MessageColor);
                return;
            }

            string name = command[0];
            if (plugin.Configuration.Instance.Kits.Exists(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
            {
                UnturnedChat.Say(caller, plugin.Translate("CreateKitExists"), plugin.MessageColor);
                return;
            }
            if (!int.TryParse(command[1], out int cooldown))
            {
                UnturnedChat.Say(caller, plugin.Translate("CreateKitInvalidCooldown"), plugin.MessageColor);
                return;
            }

            List<ushort> items = new List<ushort>();
            List<Kit.MetadataItem> metadataItems = new List<Kit.MetadataItem>();
            
            for (byte num = 0; num < PlayerInventory.PAGES - 2; num++)
            {                
                for (byte num2 = 0; num2 < player.Inventory.getItemCount(num); num2++)
                {
                    var item = player.Inventory.getItem(num, num2);
                    if (item == null)
                        continue;

                    ItemAsset asset = Assets.find(EAssetType.ITEM, item.item.id) as ItemAsset;
                    if (asset.type == EItemType.GUN)
                        metadataItems.Add(new Kit.MetadataItem(item.item.id, item.item.metadata));
                    else
                        items.Add(item.item.id);
                }
            }

            var clothing = player.Player.clothing;

            if (clothing.backpack != 0)
                items.Add(clothing.backpack);
            if (clothing.vest != 0)
                items.Add(clothing.vest);
            if (clothing.shirt != 0)
                items.Add(clothing.shirt);
            if (clothing.pants != 0)
                items.Add(clothing.pants);
            if (clothing.mask != 0)
                items.Add(clothing.mask);
            if (clothing.hat != 0)
                items.Add(clothing.hat);
            if (clothing.glasses != 0)
                items.Add(clothing.glasses);

            plugin.Configuration.Instance.Kits.Add(new Kit(name, cooldown, items, metadataItems));
            plugin.Configuration.Save();
            UnturnedChat.Say(caller, plugin.Translate("CreateKitSuccess", name, cooldown, items.Count + metadataItems.Count), plugin.MessageColor);
        }
    }
}
