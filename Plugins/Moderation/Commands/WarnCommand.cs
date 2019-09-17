using RestoreMonarchy.Moderation.Extensions;
using Rocket.API;
using Rocket.Core.Logging;
using Rocket.Unturned;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RestoreMonarchy.Moderation.Commands
{
    public class WarnCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Both;

        public string Name => "warn";

        public string Help => "Warns a player";

        public string Syntax => "<name/steamID> [reason]";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string>();

        public void Execute(IRocketPlayer caller, string[] args)
        {
            UnturnedPlayer target = args.ElementAtOrDefault(0).GetOnlinePlayer();
            string reason = args.Skip(1).GetReason();

            if (target == null)
            {
                UnturnedChat.Say(caller, U.Translate("command_generic_target_player_not_found"));
                return;
            }

            var instance = ModerationPlugin.Instance;

            string announcement = instance.Translate("WarnAnnouncement", target.CharacterName, caller.DisplayName, reason);

            UnturnedChat.Say(announcement);

            ThreadPool.QueueUserWorkItem((i) =>
            {
                try
                {
                    instance.DiscordMessager.SendMessage(new string[] { target.CharacterName, target.Id, caller.DisplayName, reason }, EMessageType.Warn);
                } catch (Exception e)
                {
                    Logger.LogException(e);
                }
                
            });
        }
    }
}
