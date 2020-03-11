using Rocket.API;
using System.Collections.Generic;
using System.Xml.Serialization;
using CustomMessageAnnouncer.Models;

namespace CustomMessageAnnouncer
{
    public class CustomMessageAnnouncerConfiguration : IRocketPluginConfiguration
    {
        public bool UseRich { get; set; }
        public string CommandsMessageColor { get; set; }
        public double MessageInterval { get; set; }
        [XmlArrayItem("Message")]
        public List<Message> Messages { get; set; }
        [XmlArrayItem("TextCommand")]
        public List<TextCommand> TextCommands { get; set; }

        public void LoadDefaults()
        {
            UseRich = false;
            CommandsMessageColor = "orange";
            MessageInterval = 180;
            Messages = new List<Message>()
            {
                new Message("You are playing on RestoreMonarchy!", "https://i.imgur.com/vWoACbH.png", "yellow"),
                new Message("Visit RestoreMonarchy.com", "https://i.imgur.com/vWoACbH.png", "yellow")
            };
            TextCommands = new List<TextCommand>()
            {
                new TextCommand("rules", "Shows rules", "orange", "There's no rules, just chill and have fun!")
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
