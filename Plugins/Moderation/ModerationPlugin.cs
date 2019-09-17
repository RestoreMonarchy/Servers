using RestoreMonarchy.Moderation.Database;
using RestoreMonarchy.Moderation.Extensions;
using RestoreMonarchy.Moderation.Models;
using Rocket.API.Collections;
using Rocket.Core.Logging;
using Rocket.Core.Plugins;
using Rocket.Core.Utils;
using Rocket.Unturned;
using Rocket.Unturned.Player;
using System;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Threading;

namespace RestoreMonarchy.Moderation
{
    public class ModerationPlugin : RocketPlugin<ModerationConfiguration>
    {
        public static ModerationPlugin Instance { get; private set; }
        public DatabaseManager DatabaseManager { get; set; }
        public DiscordMessager DiscordMessager { get; set; }
        protected override void Load()
        {
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback((sender, certificate, chain, policyErrors) => { return true; });

            U.Events.OnPlayerConnected += (player) => ThreadPool.QueueUserWorkItem((i) => ProcessPlayerJoin(player));

            DatabaseManager = new DatabaseManager(this);
            DiscordMessager = new DiscordMessager(this);
            Instance = this;

            Logger.Log($"{Name} {Assembly.GetName().Version} has been loaded!", ConsoleColor.Yellow);
        }

        protected override void Unload()
        {
            Logger.Log($"{Name} has been unloaded!", ConsoleColor.Yellow);
        }

        public override TranslationList DefaultTranslations => new TranslationList()
        {
            { "BanAnnouncement", "{0} was banned by {1} for {2} for {3}!" },
            { "BanMessage", "[BAN] Reason: {0} Time Left: {1}" },
            { "BansLimit", "You can't view the list of all bans in-game, you must log into console or use the website" },
            { "BansLine", "BanID: {0} PlayerName: {1} PlayerId: {2} PunisherName: {3} PunisherId: {4}" },
            { "BansNone", "There is no bans" },
            { "UnbanAnnouncement", "{0} was unbanned!" },
            { "UnbanFail", "{0} doesn't have any active ban." },
            { "KickAnnouncement", "{0} was kicked by {1} for {2}" },
            { "WarnAnnouncement", "{0} was warned by {1} for {2}" },
            { "CheckownerFail", "You are not looking at any barricade, structure or locked vehicle." },
            { "CheckownerSuccess", "Name: {0} | SteamID: {1} | IsBanned: {2}" }
        };

        private void ProcessPlayerJoin(UnturnedPlayer player)
        {
            Player myPlayer = DatabaseManager.GetPlayer(player.CSteamID.m_SteamID);
            
            if (myPlayer == null)
                myPlayer = DatabaseManager.CreatePlayer(new Player(player.CSteamID.m_SteamID, player.DisplayName, player.IP));

            if (myPlayer.IsBanned(out Ban ban))
            {
                TaskDispatcher.QueueOnMainThread(() => player.Kick(Translate("BanMessage", ban.BanReason.ToReason(), ban.GetTimeLeft())), 3);                
            }
        }
    }
}
