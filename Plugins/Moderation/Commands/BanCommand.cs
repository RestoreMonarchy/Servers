using RestoreMonarchy.Moderation.Models;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using RestoreMonarchy.Moderation.Extensions;
using Steamworks;
using RestoreMonarchy.Moderation.Database;
using Rocket.Core.Utils;
using Rocket.Core.Logging;
using System;

namespace RestoreMonarchy.Moderation.Commands
{
    public class BanCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Both;

        public string Name => "ban";

        public string Help => "Bans player";

        public string Syntax => "<name/steamID> [duration] [reason]";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string>();

        public void Execute(IRocketPlayer caller, string[] args)
        {
            Ban ban = new Ban();
            ban.PlayerId = args.ElementAtOrDefault(0).GetSteamID();

            if (ban.PlayerId == 0)
            {
                UnturnedChat.Say(caller, U.Translate("command_generic_failed_find_player"));
                return;
            }

            ban.PunisherId = caller.Id != "Console" ? ulong.Parse(caller.Id) : 0;
            ban.BanDuration = args.ElementAtOrDefault(1).GetDuration();
            ban.BanReason = args.Skip(2).GetReason();
            ban.BanCreated = DateTime.Now;

            Logger.Log($"Processing {ban.PlayerId} ban...", System.ConsoleColor.Cyan);

            ThreadPool.QueueUserWorkItem((i) => ProcessBan(caller, ban, ban.PlayerId.GetIP(), ban.PunisherId.GetIP()));            
        }

        private void ProcessBan(IRocketPlayer caller, Ban ban, string playerIP, string punisherIP)
        {
            try
            {
                ModerationPlugin instance = ModerationPlugin.Instance;

                ban.Player = instance.DatabaseManager.GetPlayer(ban.PlayerId);
                ban.Punisher = instance.DatabaseManager.GetPlayer(ban.PunisherId);

                if (ban.Player == null)
                    ban.Player = instance.DatabaseManager.CreatePlayer(new Player(ban.PlayerId, punisherIP));
                if (ban.Punisher == null)
                    ban.Punisher = instance.DatabaseManager.CreatePlayer(new Player(ban.PunisherId, caller.DisplayName, punisherIP));

                instance.DatabaseManager.InsertBan(ban);

                string prettyDuration = ban.BanDuration.ToPrettyTime();
                string reason = ban.BanReason.ToReason();

                TaskDispatcher.QueueOnMainThread(() =>
                {
                    UnturnedChat.Say(instance.Translate("BanAnnouncement", ban.Player.PlayerName, ban.Punisher.PlayerName, reason, prettyDuration));

                    SDG.Unturned.Provider.kick(new CSteamID(ban.PlayerId), instance.Translate("BanMessage", reason, prettyDuration));
                });

                instance.DiscordMessager.SendMessage(new string[] { ban.Player.PlayerName, ban.PlayerId.ToString(), ban.Punisher.PlayerName,
                    reason, prettyDuration }, EMessageType.Ban);

            } catch (System.Exception e)
            {
                Logger.LogException(e, "An error occurated when processing ban");
            }            
        }
    }
}
