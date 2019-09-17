using Rocket.API;
using Rocket.Core;
using Rocket.Core.Commands;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RestoreMonarchy.Moderation.Extensions
{
    public static class CommandExtension
    {
        public static ulong GetSteamID(this string value)
        {
            ulong steamId = 0;

            if (value != null)
            {
                if (!ulong.TryParse(value, out steamId) || steamId < 76561197960265728  )
                {
                    steamId = UnturnedPlayer.FromName(value)?.CSteamID.m_SteamID ?? 0;
                }
            }

            return steamId;
        }

        public static UnturnedPlayer GetOnlinePlayer(this string value)
        {
            if (value != null)
            {
                if (ulong.TryParse(value, out ulong steamId))
                {
                    return UnturnedPlayer.FromSteamPlayer(Provider.clients.FirstOrDefault(x => x.playerID.steamID.m_SteamID == steamId));
                } else
                {
                    return UnturnedPlayer.FromName(value);
                }                
            }

            return null;
        }

        public static int? GetDuration(this string value)
        {
            if (int.TryParse(value, out int result))
            {
                return result;
            }

            return null;
        }

        public static string GetReason(this IEnumerable<string> args)
        {
            if (args.Count() > 0)
                return string.Join(" ", args);
            else
                return null;
        }

        public static string GetIP(this ulong steamId)
        {
            P2PSessionState_t state;
            if (!SteamGameServerNetworking.GetP2PSessionState(new CSteamID(steamId), out state))
            {
                return null;
            }

            return Parser.getIPFromUInt32(state.m_nRemoteIP);
        }

        public static string ToPrettyTime(this int? seconds)
        {
            if (!seconds.HasValue)
                return "permanent";

            TimeSpan span = TimeSpan.FromSeconds(seconds.Value);
            return ToPrettyTime(span);
        }

        public static string ToPrettyTime(this TimeSpan span)
        {
            if (span.Seconds < 0)
                return "expired";

            string formatted = string.Format("{0}{1}{2}",
                span.Duration().Days > 0 ? string.Format("{0:0}d ", span.Days) : string.Empty,
                span.Duration().Hours > 0 ? string.Format("{0:0}h ", span.Hours) : string.Empty,
                span.Duration().Minutes > 0 ? string.Format("{0:0}m ", span.Minutes) : string.Empty);

            if (formatted.EndsWith(" ")) formatted = formatted.Substring(0, formatted.Length - 1);

            if (string.IsNullOrEmpty(formatted)) formatted = "<1m";

            return formatted;
        }

        public static string ToReason(this string value)
        {
            if (string.IsNullOrEmpty(value))
                value = @"¯\_(ツ)_/¯";

            return value;
        }
    }
}
