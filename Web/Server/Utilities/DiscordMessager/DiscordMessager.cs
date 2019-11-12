using Core.Models;
using Discord;
using Discord.Webhook;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using System.Timers;
using Web.Server.Utilities.Database;

namespace Web.Server.Utilities.DiscordMessager
{
    public class DiscordMessager
    {
        public readonly DatabaseManager database;
        private readonly string webhookUrl;
        private readonly string webhookUrl2;

        public DiscordMessager(IConfiguration configuration, DatabaseManager database)
        {
            this.database = database;
            database.onPlayerCreated += onPlayerCreatedAsync;
            webhookUrl = configuration["WebhookURL"];
            webhookUrl2 = configuration["WebhookURL2"];
            InitializePendingUnbans();
        }

        public async Task LogVerifyTaskExceptionAsync(Exception exception)
        {
            using (var client = new DiscordWebhookClient(webhookUrl)) 
            {
                EmbedBuilder eb = new EmbedBuilder();
                eb.WithColor(Color.Red);
                eb.WithDescription($"An exception occurated while verifying a payment  ```{exception.Message}```");
                eb.WithCurrentTimestamp();

                await client.SendMessageAsync(embeds: new List<Embed>() { eb.Build() });
            }
        }

        public async Task SendPayPalMessage(string msg)
        {
            using (var client = new DiscordWebhookClient(webhookUrl2))
            {
                EmbedBuilder eb = new EmbedBuilder();
                eb.WithColor(Color.Gold);
                eb.WithDescription(msg);
                eb.WithCurrentTimestamp(); ;
                
                await client.SendMessageAsync(embeds: new List<Embed>() { eb.Build() });
            }
        }

        private async Task onPlayerCreatedAsync(Player player)
        {
            string country = player.PlayerCountry != null ? $":flag_{player.PlayerCountry.ToLower()}:" : "unkown";

            using (var client = new DiscordWebhookClient(webhookUrl))
            {
                EmbedBuilder eb = new EmbedBuilder();
                eb.WithColor(Color.DarkBlue);
                eb.WithAuthor(player.PlayerName/*, $"/api/players/avatar/{playerId}"*/);
                eb.AddField("SteamID", $"[{player.PlayerId}](https://steamcommunity.com/profiles/" + player.PlayerId + ")", true);
                eb.AddField("Country", country, true);
                eb.WithTimestamp(player.PlayerCreated);

                await client.SendMessageAsync(embeds: new List<Embed>() { eb.Build() });
            }
        }

        private void InitializePendingUnbans()
        {            
            foreach (var ban in database.GetActiveBans())
            {
                var timer = new Timer((ban.ExpiryDate.Value.AddHours(1) - DateTime.Now).TotalMilliseconds);
                timer.AutoReset = false;
                timer.Elapsed += async (sender, e) => await SendUnbanWebhook(ban.PunishmentId);
                timer.Start();
            }
        }

        public async Task SendUnbanWebhook(int punishmentId)
        {
            PlayerPunishment punishment = database.GetPunishment(punishmentId);
            if (punishment == null)
                return;

            using (var client = new DiscordWebhookClient(webhookUrl))
            {
                EmbedBuilder eb = new EmbedBuilder();
                eb.WithColor(Color.Green);
                eb.WithDescription($"{punishment.Player.PlayerName}'s ban expired!");
                eb.WithTimestamp(DateTime.Now);

                await client.SendMessageAsync(embeds: new List<Embed>() { eb.Build() });
            }
        }

        public async Task SendPunishmentWebhook(int punishmentId)
        {
            PlayerPunishment punishment = database.GetPunishment(punishmentId);
            if (punishment == null)
                return;

            using (var client = new DiscordWebhookClient(webhookUrl))
            {
                EmbedBuilder eb = new EmbedBuilder();
                eb.WithColor(Color.Blue);
                eb.WithTitle($"{CultureInfo.CurrentCulture.TextInfo.ToTitleCase(punishment.Category)}");
                eb.AddField("Player", $"[{punishment.Player.PlayerName}](https://steamcommunity.com/profiles/{punishment.PlayerId})");
                eb.AddField("Punisher", $"[{punishment.Punisher.PlayerName}](https://steamcommunity.com/profiles/{punishment.PunisherId})");
                eb.AddField("Reason", punishment.Reason == null ? "unkown" : punishment.Reason);
                if (punishment.ExpiryDate.HasValue)
                    eb.AddField("Expires", punishment.ExpiryDate);
                eb.WithTimestamp(punishment.CreateDate);

                await client.SendMessageAsync(embeds: new List<Embed>() { eb.Build() });
            }
        }
    }
}
