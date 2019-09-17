using System.Xml.Serialization;

namespace RestoreMonarchy.Moderation.Models
{
    public class DiscordWebhook
    {
        public DiscordWebhook() { }

        public DiscordWebhook(string webhookType, string webhookUrl, string webhookColor, string messageFormat)
        {
            WebhookType = webhookType;
            WebhookUrl = webhookUrl;
            WebhookColor = webhookColor;
            MessageFormat = messageFormat;
        }

        [XmlAttribute]
        public string WebhookType { get; set; }
        [XmlAttribute]
        public string WebhookUrl { get; set; }
        [XmlAttribute]
        public string WebhookColor { get; set; }
        public string MessageFormat { get; set; }
    }
}
