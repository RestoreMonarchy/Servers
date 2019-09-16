using Rocket.API;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace CustomMessageAnnouncer
{
    public class CustomMessageAnnouncerConfiguration : IRocketPluginConfiguration
    {
        public double MessageInterval { get; set; }
        [XmlArrayItem("Message")]
        public List<Message> Messages { get; set; }
        public void LoadDefaults()
        {
            MessageInterval = 300;
            Messages = new List<Message>()
            {
                new Message("{size=22}You are playing on RestoreMonarchy!{/size}", "https://i.imgur.com/vWoACbH.png", "yellow"),
                new Message("{size=22}Visit RestoreMonarchy.com{/size}", "https://i.imgur.com/vWoACbH.png", "yellow")
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
