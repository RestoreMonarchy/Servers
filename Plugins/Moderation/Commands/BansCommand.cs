using RestoreMonarchy.Moderation.Database;
using RestoreMonarchy.Moderation.Extensions;
using RestoreMonarchy.Moderation.Models;
using Rocket.API;
using Rocket.Core.Utils;
using Rocket.Unturned.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RestoreMonarchy.Moderation.Commands
{
    public class BansCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Both;

        public string Name => "bans";

        public string Help => "List of bans";

        public string Syntax => "[steamID]";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string>();

        public void Execute(IRocketPlayer caller, string[] args)
        {
            ulong playerId = args.ElementAtOrDefault(0).GetSteamID();

            if (caller.Id != "Console" && playerId == 0)
            {
                UnturnedChat.Say(caller, ModerationPlugin.Instance.Translate("BansLimit"));
                return;
            }

            ThreadPool.QueueUserWorkItem((i) => ProcessBans(caller, playerId));
        }

        private void ProcessBans(IRocketPlayer caller, ulong playerId)
        {
            var instance = ModerationPlugin.Instance;

            List<Ban> bans = instance.DatabaseManager.GetAllBans();

            if (playerId != 0)
                bans = bans.Where(x => x.PlayerId == playerId).ToList();

            TaskDispatcher.QueueOnMainThread(() =>
            {
                if (bans.Count == 0)
                {
                    UnturnedChat.Say(caller, instance.Translate("BansNone"));
                    return;
                }

                foreach (var ban in bans)
                {
                    UnturnedChat.Say(caller, instance.Translate("BansLine", ban.BanId, ban.Player.PlayerName, ban.PlayerId, ban.Punisher.PlayerName,
                        ban.PunisherId, ban.BanReason.ToReason(), ban.BanDuration.ToPrettyTime()));
                }
            });
        }
    }
}
