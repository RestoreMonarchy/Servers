using Rocket.API;
using Rocket.Unturned.Chat;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace CustomMessageAnnouncer.Models
{
    public class RocketTextCommand : IRocketCommand
    {        
        public RocketTextCommand(string name, string help, string color, string message)
        {
            Name = name;
            Help = help;
            Color = color;
            Message = message;
        }

        public AllowedCaller AllowedCaller => AllowedCaller.Both;

        public string Name { get; set; }
        public string Help { get; set; }
        public string Color { get; set; }
        public string Message { get; set; }

        public string Syntax => "";
        public List<string> Aliases => new List<string>();
        public List<string> Permissions => new List<string>();

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedChat.Say(caller, Message, UnturnedChat.GetColorFromName(Color, UnityEngine.Color.green));
        }
    }
}
