using Rocket.API;
using Rocket.API.Collections;
using Rocket.Core.Commands;
using Rocket.Core.Plugins;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System.Linq;
using System.Timers;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;

namespace UsefulCommands
{
    public class UsefulCommandsPlugin : RocketPlugin
    {
        protected override void Load()
        {
            Logger.Log($"{Name} {Assembly.GetName().Version} has been loaded!");
        }

        protected override void Unload()
        {
            Logger.Log($"{Name} has been unloaded!");
        }

        [RocketCommand("maxskills", "Gives you maxskills", AllowedCaller: AllowedCaller.Player)]
        public void MaxskillsCommand(IRocketPlayer caller, string[] args)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;
            player.MaxSkills();
            UnturnedChat.Say(caller, Translate("MaxskillsSuccess"));
        }

        [RocketCommand("restart", "Restarts the server within the time and optionally tells the reason", "<time> [reason]", AllowedCaller.Both)]
        public void RestartCommand(IRocketPlayer caller, string[] args)
        {
            Color color = Color.red;

            if (args.Length == 0 || !double.TryParse(args[0], out double time))
            {
                UnturnedChat.Say(caller, Translate("RestartFormat"));
                return;
            }

            string reason = string.Join(" ", args.Skip(1));

            UnturnedChat.Say(Translate("RestartNotify", time), color);
            Timer timer = new Timer(1000);
            timer.Elapsed += (o, e) =>
            {
                if (time != 0 && (time % 10 == 0 || (time <= 30 && time % 5 == 0)))
                {
                    UnturnedChat.Say(Translate("RestartNotify", time), color);
                }
                
                if (--time == 0)
                {
                    UnturnedChat.Say(Translate("RestartSuccess"), color);
                    SaveManager.save();
                    Provider.shutdown(0, reason);
                    timer.Enabled = false;
                }
            };
            timer.Start();
        }

        public override TranslationList DefaultTranslations => new TranslationList()
        {
            { "MaxskillsSuccess", "Successfully given you maxskills!" },
            { "RestartFormat", "Format: /restart <time> [reason]" },
            { "RestartNotify", "Server restart in {0} seconds!" },
            { "RestartSuccess", "Server is restarting..." }
        };
    }
}
