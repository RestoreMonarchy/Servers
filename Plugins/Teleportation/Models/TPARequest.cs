﻿using Rocket.Core.Utils;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Teleportation.Models
{
    public class TPARequest
    {
        public TPARequest(CSteamID sender, CSteamID target)
        {
            Sender = sender;
            Target = target;
        }

        public TPARequest() { }

        public CSteamID Sender { get; set; }
        public CSteamID Target { get; set; }

        public void Execute(double delay)
        {
            var plugin = TeleportationPlugin.Instance;
            var sender = UnturnedPlayer.FromCSteamID(Sender);
            var target = UnturnedPlayer.FromCSteamID(Target);
            
            if (delay > 0)
            {
                UnturnedChat.Say(Sender, plugin.Translate("TPADelay", target.DisplayName, delay), plugin.MessageColor);
            }

            TaskDispatcher.QueueOnMainThread(() =>
            {
                if (!plugin.Configuration.Instance.AllowCombat && plugin.CombatPlayers.TryGetValue(Sender.m_SteamID, out Timer timer) && timer.Enabled)
                {
                    UnturnedChat.Say(Sender, plugin.Translate("TPAWhileCombat", target.DisplayName), plugin.MessageColor);
                    UnturnedChat.Say(Target, plugin.Translate("TPAWhileCombat", sender.DisplayName), plugin.MessageColor);
                    return;
                } else if (!plugin.Configuration.Instance.AllowRaid && plugin.RaidPlayers.TryGetValue(Sender.m_SteamID, out Timer timer2) && timer2.Enabled)
                {
                    UnturnedChat.Say(Sender, plugin.Translate("TPAWhileRaid", target.DisplayName), plugin.MessageColor);
                    UnturnedChat.Say(Target, plugin.Translate("TPAWhileRaid", sender.DisplayName), plugin.MessageColor);
                    return;
                } else if (sender.Dead || target.Dead)
                {
                    UnturnedChat.Say(Sender, plugin.Translate("TPADead", target.DisplayName), plugin.MessageColor);
                    UnturnedChat.Say(Target, plugin.Translate("TPADead", sender.DisplayName), plugin.MessageColor);
                } else if (!plugin.Configuration.Instance.AllowCave && LevelGround.checkSafe(target.Position) != target.Position)
                {
                    UnturnedChat.Say(Sender, plugin.Translate("TPACave", target.DisplayName), plugin.MessageColor);
                    UnturnedChat.Say(Target, plugin.Translate("TPACave", target.DisplayName), plugin.MessageColor);
                }
                else
                {
                    sender.Teleport(target);
                    UnturnedChat.Say(Sender, plugin.Translate("TPASuccess", target.DisplayName), plugin.MessageColor);
                }

            }, (float)delay);
        }
    }
}
