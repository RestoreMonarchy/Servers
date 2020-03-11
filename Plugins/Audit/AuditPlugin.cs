using Core.Models;
using Rocket.Core.Plugins;
using Rocket.Unturned;
using System;
using System.Collections.Generic;
using RestoreMonarchy.Audit.Utilities;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;
using Rocket.Unturned.Chat;
using Rocket.API.Collections;
using System.Net;
using Rocket.Core;
using RestoreMonarchy.Audit.Providers;

namespace RestoreMonarchy.Audit
{
    public class AuditPlugin : RocketPlugin<AuditConfiguration>
    {
        public Color MessageColor { get; set; }
        public static AuditPlugin Instance { get; private set; }
        public List<Player> PlayersCache { get; set; }
        public WebClient Client 
        { 
            get
            {
                WebClient wc = new WebClient();
                wc.Headers.Add("x-api-key", Configuration.Instance.APIKey);
                return wc;
            }
        }

        protected override void Load()
        {
            Instance = this;
            MessageColor = UnturnedChat.GetColorFromName(Configuration.Instance.MessageColor, Color.green);
            PlayersCache = new List<Player>();

            var permissionsProvider = new PermissionsProvider(Instance);
            permissionsProvider.Initialize();
            R.Permissions = permissionsProvider;
            
            U.Events.OnPlayerConnected += Instance.OnPlayerConnected;

            Logger.Log($"{Name} {Assembly.GetName().Version} has been loaded!", ConsoleColor.Yellow);
        }

        protected override void Unload()
        {
            U.Events.OnPlayerConnected -= Instance.OnPlayerConnected;
            Logger.Log($"{Name} has been unloaded!", ConsoleColor.Yellow);
        }

        public override TranslationList DefaultTranslations => new TranslationList()
        {
            { "JoinMessage", "[{0}] {1} has joined the server from {2}!" }
        };
    }
}
