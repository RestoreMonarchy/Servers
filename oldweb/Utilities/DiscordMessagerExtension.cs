using Core.Models;
using Core.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace RestoreMonarchy.WebAPI.Utilities
{
    public static class DiscordMessagerExtension
    {
        public static void SendBanWebhook(this DiscordMessager messager, int banId)
        {
            PlayerBan ban = messager.Database.GetPlayerBan(banId);
            
            if (ban != null)
            {
                DiscordWebhookMessage msg = new DiscordWebhookMessage();
                string playerCountry = ban.Player.PlayerCountry != null ? $":flag_{ban.Player.PlayerCountry.ToLower()}:" : string.Empty;
                string punisherCountry = ban.Punisher.PlayerCountry != null ? $":flag_{ban.Punisher.PlayerCountry.ToLower()}:" : string.Empty;
                string desc = $"**[{ban.Player.PlayerName}](https://steamcommunity.com/profiles/{ban.Player.PlayerId})** {playerCountry} was banned by " +
                    $"**[{ban.Punisher.PlayerName}](https://steamcommunity.com/profiles/{ban.Punisher.PlayerId})** {punisherCountry} with reason `{ban.BanReason}` " +
                    $"for `{ban.BanDuration}` sec";
                var embed = new DiscordWebhookMessage.Embed(desc, 123);
                msg.embeds.Add(embed);
                messager.SendMessage(msg);
            }
        }

        public static void SendUnbanWebhook(this DiscordMessager messager, int banId)
        {
            PlayerBan ban = messager.Database.GetPlayerBan(banId);
            SendUnbanWebhook(messager, ban);
        }

        public static void SendUnbanWebhook(this DiscordMessager messager, PlayerBan ban)
        {
            if (ban != null)
            {
                DiscordWebhookMessage msg = new DiscordWebhookMessage();
                string desc = $"**[{ban.Player.PlayerName}](https://steamcommunity.com/profiles/{ban.Player.PlayerId})'s** `{ban.BanDuration}` sec ban expired";
                var embed = new DiscordWebhookMessage.Embed(desc, 12582656);
                msg.embeds.Add(embed);
                messager.SendMessage(msg);
            }
        }

        public static void SendPlayerCreatedWebhook(this DiscordMessager messager, ulong playerId)
        {
            SendPlayerCreatedWebhook(messager, messager.Database.GetPlayer(playerId));
        }

        public static void SendPlayerCreatedWebhook(this DiscordMessager messager, Player player)
        {
            if (player != null)
            {
                DiscordWebhookMessage msg = new DiscordWebhookMessage();
                var embed = new DiscordWebhookMessage.Embed();
                embed.color = 16776960;
                string playerCountry = player.PlayerCountry != null ? $":flag_{player.PlayerCountry.ToLower()}:" : "unkown";
                embed.title = "New Player Created"; 
                embed.fields = new List<DiscordWebhookMessage.Embed.Field>()
                {
                    new DiscordWebhookMessage.Embed.Field("Name", $"**[{player.PlayerName}](https://steamcommunity.com/profiles/{player.PlayerId})**", true),
                    new DiscordWebhookMessage.Embed.Field("SteamID", $"{player.PlayerId}", true),
                    new DiscordWebhookMessage.Embed.Field("Country", playerCountry, true)
                };
                msg.embeds.Add(embed);
                messager.SendMessage(msg);
            }
        }
    }
}
