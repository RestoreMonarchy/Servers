using Kits.Models;
using Rocket.API.Collections;
using Rocket.Core.Plugins;
using Rocket.Unturned.Chat;
using System;
using System.Collections.Generic;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;

namespace Kits
{
    public class KitsPlugin : RocketPlugin<KitsConfiguration>
    {
        public static KitsPlugin Instance { get; private set; }
        public List<KitCooldown> Cooldowns { get; set; }
        public Color MessageColor { get; set; }

        protected override void Load()
        {
            Instance = this;            
            MessageColor = UnturnedChat.GetColorFromName(Configuration.Instance.MessageColor, Color.green);
            Cooldowns = new List<KitCooldown>();

            Logger.Log($"{Name} {Assembly.GetName().Version} has been loaded!", ConsoleColor.Yellow);
        }

        protected override void Unload()
        {
            Logger.Log($"{Name} has been unloaded!", ConsoleColor.Yellow);
        }

        public override TranslationList DefaultTranslations => new TranslationList()
        {
            { "CreateKitHelp", "Use: /ckit <name> <cooldown> [experience]" },
            { "CreateKitExists", "The kit with such name already exists" },
            { "CreateKitInvalidCooldown", "Cooldown is in incorrect format" },
            { "CreateKitSuccess", "Successfully created kit {0} cooldown {1} with {2} items" },
            { "CooldownExpiredNotify", "Cooldown for kit {0} expired" },
            { "Kits", "Yours kits:" },
            { "KitHelp", "Use: /kit <name>" },
            { "KitNotFound", "Failed to find any kit with such name" },
            { "KitCooldown", "You have to wait {0} before you can use this kit again" },
            { "KitSuccess", "You received kit {0}" }
        };
    }
}
