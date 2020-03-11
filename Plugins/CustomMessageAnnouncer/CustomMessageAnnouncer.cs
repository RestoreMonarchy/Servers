using CustomMessageAnnouncer.Models;
using Rocket.API.Collections;
using Rocket.Core;
using Rocket.Core.Plugins;
using Rocket.Unturned.Chat;
using SDG.Unturned;
using System.Timers;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;

namespace CustomMessageAnnouncer
{
    public class CustomMessageAnnouncer : RocketPlugin<CustomMessageAnnouncerConfiguration>
    {
        private Timer timer;
        private int index = 0;

        public static CustomMessageAnnouncer Instance { get; private set; }

        protected override void Load()
        {
            Instance = this;
            timer = new Timer(Configuration.Instance.MessageInterval * 1000);
            timer.AutoReset = true;
            timer.Elapsed += SendMessage;
            timer.Start();
            
            foreach (var textCommand in Configuration.Instance.TextCommands)
            {
                var cmd = new RocketTextCommand(textCommand.Name, textCommand.Help, textCommand.Color, textCommand.Message);
                R.Commands.Register(cmd);
            }

            Logger.Log($"{Name} {Assembly.GetName().Version} has been loaded!");
        }

        public override TranslationList DefaultTranslations => new TranslationList()
        {
            { "Commands", "Your commands: {0}" }
        };

        protected override void Unload()
        {
            Logger.Log($"{Name} has been unloaded!");
        }

        private void SendMessage(object sender, ElapsedEventArgs e)
        {
            if (index >= Configuration.Instance.Messages.Count)
            {
                index = 0;
            }

            var msg = Configuration.Instance.Messages[index];

            if (Configuration.Instance.UseRich)
            {
                ChatManager.serverSendMessage(msg.Text.Replace('{', '<').Replace('}', '>'), 
                    UnturnedChat.GetColorFromName(msg.Color, Color.green), null, null, EChatMode.SAY, msg.IconUrl, true);
            } else
            {
                UnturnedChat.Say(msg.Text, UnturnedChat.GetColorFromName(msg.Color, Color.green));
            }
            index++;
        }
    }
}
