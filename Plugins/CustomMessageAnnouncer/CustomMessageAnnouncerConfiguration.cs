using Rocket.API;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace CustomMessageAnnouncer
{
    public class CustomMessageAnnouncerConfiguration : IRocketPluginConfiguration
    {
        public bool UseRich { get; set; }
        public double MessageInterval { get; set; }
        [XmlArrayItem("Message")]
        public List<Message> Messages { get; set; }
        public void LoadDefaults()
        {
            UseRich = false;
            MessageInterval = 180;
            Messages = new List<Message>()
            {
                new Message("You are playing on RestoreMonarchy!", "https://i.imgur.com/vWoACbH.png", "yellow"),
                new Message("Visit RestoreMonarchy.com", "https://i.imgur.com/vWoACbH.png", "yellow")
            };
        }
    }

    public sealed class Message
    {
        public Message(string text, string iconUrl, string color)
        {
            Text = text;
            IconUrl = iconUrl;
            Color = color;
        }

        public Message() { }

        [XmlAttribute]
        public string Text { get; set; }
        [XmlAttribute]
        public string IconUrl { get; set; }
        [XmlAttribute]
        public string Color { get; set; }
    }
}
