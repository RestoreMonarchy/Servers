using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Core.Utilities
{
    public class DiscordWebhookMessage
    {
        public DiscordWebhookMessage()
        {
            embeds = new List<Embed>();
        }

        public List<Embed> embeds { get; set; }
        public class Embed
        {
            public Embed(string description, int color)
            {
                this.description = description;
                this.color = color;
                this.timestamp = DateTime.UtcNow.ToString("u");
            }

            public Embed()
            {
                this.timestamp = DateTime.UtcNow.ToString("u");
            }

            public string title { get; set; }
            public string description { get; set; }
            public int color { get; set; }
            public string timestamp { get; set; }
            public List<Field> fields { get; set; }

            public class Field
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
    }
}
