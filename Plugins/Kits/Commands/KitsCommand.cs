using Rocket.API;
using Rocket.Unturned.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kits.Commands
{
    public class KitsCommand : IRocketCommand
    {
        private KitsPlugin pluginInstance => KitsPlugin.Instance;
        public void Execute(IRocketPlayer caller, string[] command)
        {
            var permissions = caller.GetPermissions();

            StringBuilder sb = new StringBuilder(pluginInstance.Translate("Kits"));
            var playerKits = pluginInstance.KitsCache.Where(kit => caller.IsAdmin || 
                permissions.Exists(x => x.Name.Equals("kit." + kit.Name, StringComparison.OrdinalIgnoreCase))).ToList();
            
            if (playerKits.Count < 1)
            {
                UnturnedChat.Say(caller, pluginInstance.Translate("NoKits"), pluginInstance.MessageColor);
                return;
            }

            foreach (var kit in playerKits)
            {
                string cooldownString = string.Empty;
                var cooldown = pluginInstance.Cooldowns.FirstOrDefault(x => x.Kit.Name == kit.Name && x.Player.Id == caller.Id);
                if (cooldown != null && cooldown.Timer.Enabled)
                    cooldownString = " [" + (kit.Cooldown - (DateTime.Now - cooldown.TimeStarted).TotalSeconds).ToString("0") + "s]";

                sb.Append($" {kit.Name}{cooldownString},");
            }

            UnturnedChat.Say(caller, sb.ToString().TrimEnd(','), pluginInstance.MessageColor);
        }

        public AllowedCaller AllowedCaller => AllowedCaller.Both;

        public string Name => "kits";

        public string Help => "Lists all kits available for you";

        public string Syntax => "";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string>();
    }
}
