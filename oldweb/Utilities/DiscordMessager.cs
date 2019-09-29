using Core.Utilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RestoreMonarchy.WebAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace RestoreMonarchy.WebAPI.Utilities
{
    public class DiscordMessager
    {
        private readonly string webhookUrl;
        private readonly WebClient webClient;

        public Database Database { get; set; }

        public DiscordMessager(IConfiguration configuration, Database database)
        {
            Database = database;
            webhookUrl = configuration["WebhookURL"];
            webClient = new WebClient();
            webClient.Headers.Add(HttpRequestHeader.ContentType, "application/json");
        }

        public void SendMessage(DiscordWebhookMessage message)
        {
            try
            {
                webClient.UploadString(webhookUrl, JsonConvert.SerializeObject(message));
            } catch (Exception e)
            {
                Console.WriteLine(e);
            }            
        }
    }
}
