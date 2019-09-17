using Newtonsoft.Json;
using RestoreMonarchy.Moderation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace RestoreMonarchy.Moderation.Extensions
{
    public class DiscordMessager
    {
        private readonly ModerationPlugin pluginInstance;
        private ModerationConfiguration configuration => pluginInstance.Configuration.Instance;

        public DiscordMessager(ModerationPlugin pluginInstance)
        {
            this.pluginInstance = pluginInstance;
        }

        public void SendMessage(string content, EMessageType messageType)
        {
            DiscordWebhook webhook = configuration.Webhooks.FirstOrDefault(x => x.WebhookType == messageType.ToString());

            if (webhook == null || string.IsNullOrEmpty(webhook.WebhookUrl))
                return;

            DiscordWebhookMessage msg = new DiscordWebhookMessage();
            msg.embeds = new List<Embed>() { new Embed(content, Convert.ToInt32(webhook.WebhookColor, 16)) };

            using (WebClient wc = new WebClient())
            {
                wc.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                wc.UploadString(webhook.WebhookUrl, JsonConvert.SerializeObject(msg));
            }
        }

        public void SendMessage(string[] args, EMessageType messageType)
        {
            DiscordWebhook webhook = configuration.Webhooks.FirstOrDefault(x => x.WebhookType == messageType.ToString());

            if (webhook == null || string.IsNullOrEmpty(webhook.WebhookUrl))
                return;

            List<Field> fields = new List<Field>();

            string[] array = webhook.MessageFormat.Split(new string[] { ": ", ", " }, StringSplitOptions.RemoveEmptyEntries);
            int num = 0;

            while (num < array.Length - 1)
            {
                string[] arr = array.Skip(num).Take(2).ToArray();

                string value = arr[1].Replace("{name}", args[0]).Replace("{steamid}", args[1]).Replace("{punisher}", args[2]);

                if (messageType != EMessageType.Unban)
                    value = value.Replace("{reason}", args[3]);
                if (messageType == EMessageType.Ban)
                    value = value.Replace("{duration}", args[4]);

                Field field = new Field(arr[0], value, true);

                fields.Add(field);
                num += 2;
            }

            DiscordWebhookMessage msg = new DiscordWebhookMessage();

            msg.embeds = new List<Embed>() { new Embed(fields) };

            using (WebClient wc = new WebClient())
            {
                wc.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                wc.UploadString(webhook.WebhookUrl, JsonConvert.SerializeObject(msg));
            }
        }

        private class DiscordWebhookMessage
        {
            public List<Embed> embeds { get; set; }
        }

        private class Embed
        {
            public Embed(string title, int color)
            {
                this.title = title;
                this.color = color;
                this.timestamp = DateTime.UtcNow.ToString("u");
            }

            public Embed(List<Field> fields)
            {
                this.fields = fields;
                this.timestamp = DateTime.UtcNow.ToString("u");
            }

            public string title { get; set; }
            public int color { get; set; }
            public string timestamp { get; set; }
            public List<Field> fields { get; set; }
        }

        private class Field
        {
            public Field(string name, string value, bool inline)
            {
                this.name = name;
                this.value = value;
                this.inline = inline;
            }

            public string name { get; set; }
            public string value { get; set; }
            public bool inline { get; set; }
        }
    }

    public enum EMessageType
    {
        Ban,
        Kick,
        Warn,
        Unban
    }
}
