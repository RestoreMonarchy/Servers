using Rocket.API.Collections;
using Rocket.Core.Logging;
using Rocket.Core.Plugins;
using Rocket.Unturned.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;

namespace Kits
{
    public class KitsPlugin : RocketPlugin<KitsConfiguration>
    {
        public static KitsPlugin Instance { get; private set; }
        public Color MessageColor { get; set; }

        protected override void Load()
        {
            Instance = this;
            MessageColor = UnturnedChat.GetColorFromName(Configuration.Instance.MessageColor, Color.green);
            Logger.Log($"{Name} {Assembly.GetName().Version} has been loaded!", ConsoleColor.Yellow);
        }

        protected override void Unload()
        {
            Logger.Log($"{Name} has been unloaded!", ConsoleColor.Yellow);
        }

        public override TranslationList DefaultTranslations => new TranslationList()
        {
            { "CreateKitHelp", "Use: /ckit <name> <cooldown> [experience]" },
            { "CreateKitInvalidCooldown", "Cooldown is in incorrect format" },
            { "CreateKitSuccess", "Successfully created kit {0} cooldown {1} with {2} items" }
        };
    }
}
