using RestoreMonarchy.Moderation.Models;
using Rocket.API;
using System.Collections.Generic;

namespace RestoreMonarchy.Moderation
{
    public class ModerationConfiguration : IRocketPluginConfiguration
    {
        public string ConnectionString { get; set; }
        public List<DiscordWebhook> Webhooks { get; set; }
        public double RefreshTime { get; set; }

        public void LoadDefaults()
        {
            ConnectionString = "Server=localhost;Database=unturned;Uid=root;Password=Password123;";
            Webhooks = new List<DiscordWebhook>()
            {
                new DiscordWebhook("Ban", "", "FF0000", "Name: {name}, SteamID: {steamid}, Punisher: {punisher}, Duration: {duration}, Reason: {reason}"),
                new DiscordWebhook("Kick", "", "F06C00", "Name: {name}, SteamID: {steamid}, Punisher: {punisher}, Reason: {reason}" ),
                new DiscordWebhook("Warn", "", "FFFF00", "Name: {name}, SteamID: {steamid} Punisher: {punisher}, Reason: {reason}"),
                new DiscordWebhook("Unban", "", "00ff33", "Name: {name}, SteamID: {steamid}, Unbanner: {punisher}" )
            };
            RefreshTime = 50000;
        }        
    }
}