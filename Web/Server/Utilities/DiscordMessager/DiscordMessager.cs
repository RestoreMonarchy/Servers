using Core.Utilities;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Web.Server.Utilities.DiscordMessager
{
    public class DiscordMessager
    {
        private readonly string webhookUrl;
        private readonly WebClient webClient;
        public DiscordUnbanNotifier Notifier { get; set; }

        public Database Database { get; set; }

        public DiscordMessager(IConfiguration configuration, Database database)
        {
            Database = database;
            webhookUrl = configuration["WebhookURL"];
            webClient = new WebClient();
            webClient.Headers.Add(HttpRequestHeader.ContentType, "application/json");
            Notifier = new DiscordUnbanNotifier(database, this);
        }

        public void SendMessage(DiscordWebhookMessage message)
        {
            try
            {
                webClient.UploadString(webhookUrl, JsonConvert.SerializeObject(message));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
