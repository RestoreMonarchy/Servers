﻿using Core.Models;
using System;
using System.Collections.Generic;
using System.Timers;
using Web.Server.Utilities.Database;

namespace Web.Server.Utilities.DiscordMessager
{
    public delegate void PlayerBanAdded(int banId);
    public class DiscordUnbanNotifier
    {
        public event PlayerBanAdded OnPlayerBanAdded;

        public Dictionary<int, Timer> ActiveBans { get; private set; }
        private readonly DatabaseManager database;
        private readonly DiscordMessager messager;
        public DiscordUnbanNotifier(DatabaseManager database, DiscordMessager messager)
        {
            this.database = database;
            this.messager = messager;
            ActiveBans = new Dictionary<int, Timer>();
            Initialize();
        }

        private void Initialize()
        {
            List<PlayerBan> bans = database.GetActivePlayerBans();

            foreach (var ban in bans)
            {
                TryAddPlayerBan(ban);
            }

            OnPlayerBanAdded += onPlayerBanAdded;
        }

        private void onPlayerBanAdded(int banId)
        {
            PlayerBan ban = database.GetPlayerBan(banId);
            TryAddPlayerBan(ban);
        }

        private void TryAddPlayerBan(PlayerBan ban)
        {
            if (ban != null && ban.BanDuration.HasValue)
            {
                double milisecondsLeft = (ban.BanCreated.AddSeconds((double)ban.BanDuration.Value) - DateTime.Now).TotalMilliseconds;
                if (milisecondsLeft < 100)
                    OnBanExpire(ban);
                else
                {
                    Timer timer = new Timer(milisecondsLeft);
                    timer.AutoReset = false;
                    timer.Elapsed += (sender, e) => OnBanExpire(ban);
                    ActiveBans.TryAdd(ban.BanId, timer);
                    timer.Start();
                }
            }
        }

        private void OnBanExpire(PlayerBan ban)
        {
            messager.SendUnbanWebhook(ban);
            database.SetPlayerBanFlag(ban.BanId, true);
            ActiveBans.Remove(ban.BanId);
        }
    }
}
