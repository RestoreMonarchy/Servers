using Rocket.Core.Commands;
using Rocket.Core.Plugins;
using Rocket.Unturned.Chat;
using SDG.Unturned;
using System.Timers;
using UnityEngine;

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
        }

        private void SendMessage(object sender, ElapsedEventArgs e)
        {
            if (index >= Configuration.Instance.Messages.Count)
            {
                index = 0;
            }
            
            ChatManager.serverSendMessage(Configuration.Instance.Messages[index].Text.Replace('{', '<').Replace('}', '>'), 
                UnturnedChat.GetColorFromName(Configuration.Instance.Messages[index].Color, Color.green), 
                iconURL: Configuration.Instance.Messages[index].IconUrl, useRichTextFormatting: true);
            index++;
        }
    }
}
