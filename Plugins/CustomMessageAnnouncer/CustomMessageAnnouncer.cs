using Rocket.Core.Plugins;
using Rocket.Unturned;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System;
using System.Net;
using System.Timers;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;

namespace CustomMessageAnnouncer
{
    public class CustomMessageAnnouncer : RocketPlugin<CustomMessageAnnouncerConfiguration>
    {
        private Timer timer;
        private int index = 0;

        protected override void Load()
        {
            timer = new Timer(Configuration.Instance.MessageInterval * 1000);
            timer.AutoReset = true;
            timer.Elapsed += SendMessage;
            timer.Start();

            Logger.Log($"{Name} {Assembly.GetName().Version} has been loaded!");
        }

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
            ChatManager.serverSendMessage(msg.Text.Replace('{', '<').Replace('}', '>'), UnturnedChat.GetColorFromName(msg.Color, Color.green), 
                iconURL: msg.IconUrl, useRichTextFormatting: Configuration.Instance.UseRich);
            
            index++;
        }
    }
}
