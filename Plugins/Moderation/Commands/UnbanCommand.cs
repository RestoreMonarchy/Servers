using RestoreMonarchy.Moderation.Database;
using RestoreMonarchy.Moderation.Extensions;
using RestoreMonarchy.Moderation.Models;
using Rocket.API;
using Rocket.Core.Logging;
using Rocket.Core.Utils;
using Rocket.Unturned;
using Rocket.Unturned.Chat;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace RestoreMonarchy.Moderation.Commands
{
    public class UnbanCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Both;

        public string Name => "unban";

        public string Help => "Unbans a player";

        public string Syntax => "[steamID]";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string>();

        public void Execute(IRocketPlayer caller, string[] args)
        {
            ulong playerId = args.ElementAtOrDefault(0).GetSteamID();

            if (playerId == 0)
            {
                UnturnedChat.Say(caller, U.Translate("command_generic_failed_find_player"));
                return;
            }

            ThreadPool.QueueUserWorkItem((i) => ProcessUnban(caller, playerId));
        }

        private void ProcessUnban(IRocketPlayer caller, ulong playerId)
        {
            try
            {
                var instance = ModerationPlugin.Instance;

                Player player = instance.DatabaseManager.GetPlayer(playerId);

                string message;

                if (player == null)
                {
                    message = U.Translate("command_generic_failed_find_player");
                }
                else if (player.IsBanned(out Ban ban))
                {
                    instance.DatabaseManager.RemoveBan(ban.BanId);
                    message = instance.Translate("UnbanAnnouncement", player.PlayerName, caller.DisplayName);
                    instance.DiscordMessager.SendMessage(new string[] { player.PlayerName, player.PlayerId.ToString(), caller.DisplayName }, EMessageType.Unban);
                }
                else
                {
                    message = instance.Translate("UnbanFail", player.PlayerName);
                }

                TaskDispatcher.QueueOnMainThread(() =>
                {
                    UnturnedChat.Say(caller, message);
                });

            } catch (System.Exception e)
            {
                Logger.LogException(e);
            }            
        }
    }
}
