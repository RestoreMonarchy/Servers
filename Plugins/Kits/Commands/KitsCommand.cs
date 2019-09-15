using Rocket.API;
using Rocket.Unturned.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kits.Commands
{
    public class KitsCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Both;

        public string Name => "kits";

        public string Help => "Lists all kits available for you";

        public string Syntax => "";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string>();

        public void Execute(IRocketPlayer caller, string[] command)
        {
            var plugin = KitsPlugin.Instance;
            var permissions = caller.GetPermissions();

            StringBuilder sb = new StringBuilder(plugin.Translate("Kits"));

            foreach (var kit in plugin.Configuration.Instance.Kits)
            {
                if (caller.IsAdmin || permissions.Exists(x => x.Name.Equals("kit." + kit.Name, StringComparison.OrdinalIgnoreCase)))
                {
                    string cooldownString = string.Empty;
                    var cooldown = plugin.Cooldowns.FirstOrDefault(x => x.Kit.Name == kit.Name && x.Player.Id == caller.Id);
                    if (cooldown != null && cooldown.Timer.Enabled)
                        cooldownString = '[' + (kit.Cooldown - (DateTime.Now - cooldown.TimeStarted).TotalSeconds).ToString("0") + ']';

                    sb.Append($" {kit.Name}{cooldownString},");
                }                
            }

            UnturnedChat.Say(caller, sb.ToString().TrimEnd(','), plugin.MessageColor);
        }
    }
}
