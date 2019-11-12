using Core.Models;
using Newtonsoft.Json;
using Rocket.Core.Logging;
using Rocket.Core.Utils;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System;
using System.Linq;
using System.Net;
using System.Threading;

namespace RestoreMonarchy.Audit.Utilities
{
    public static class AuditPlayersExtension
    {
        public static void OnPlayerConnected(this AuditPlugin plugin, UnturnedPlayer player)
        {
            ThreadPool.QueueUserWorkItem((i) =>
            {
                try
                {
                    Player corePlayer = plugin.PlayersCache.FirstOrDefault(x => x.PlayerId == player.Id);
                    if (corePlayer == null)
                    {
                        using (WebClient wc = plugin.Client)
                        {
                            string response = wc.DownloadString(plugin.Configuration.Instance.APIUrl + "/players/" + player.Id + "?ip=" + player.IP);
                            corePlayer = JsonConvert.DeserializeObject<Player>(response);
                        }   

                        plugin.PlayersCache.Add(corePlayer);
                    }
                    TaskDispatcher.QueueOnMainThread(() =>
                    {
                        UnturnedChat.Say(plugin.Translate("JoinMessage", corePlayer.Role, corePlayer.PlayerName), plugin.MessageColor);
                    });
                }
                catch (Exception e)
                {
                    Logger.LogException(e);
                }
            });
        }
    }
}
