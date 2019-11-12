using Core.Models;
using Kits.Models;
using Rocket.API.Collections;
using Rocket.Core.Plugins;
using Rocket.Unturned.Chat;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using UnityEngine;
using Newtonsoft.Json;
using Logger = Rocket.Core.Logging.Logger;
using System.Net;
using Rocket.Core.Commands;
using Rocket.API;

namespace Kits
{
    public class KitsPlugin : RocketPlugin<KitsConfiguration>
    {
        public static KitsPlugin Instance { get; private set; }
        public List<Kit> KitsCache { get; private set; }
        public List<KitCooldown> Cooldowns { get; set; }
        public Color MessageColor { get; set; }

        protected override void Load()
        {
            Instance = this;
            MessageColor = UnturnedChat.GetColorFromName(Configuration.Instance.MessageColor, Color.green);            
            Cooldowns = new List<KitCooldown>();
            GetKits();
            Logger.Log($"{Name} {Assembly.GetName().Version} has been loaded!", ConsoleColor.Yellow);
        }

        [RocketCommand("refreshkits", "Refreshes the kits")]
        public void RefreshKits(IRocketPlayer caller, params string[] command)
        {
            GetKits();
            UnturnedChat.Say(caller, Translate("RefreshKitsPending"), MessageColor);
        }

        public void GetKits()
        {
            ThreadPool.QueueUserWorkItem((i) =>
            {
                try
                {
                    string response;
                    using (WebClient wc = new WebClient())
                    {
                        wc.Headers.Add("x-api-key", Configuration.Instance.APIKey);
                        response = wc.DownloadString(Configuration.Instance.APIUrl);
                    }
                    KitsCache = JsonConvert.DeserializeObject<List<Kit>>(response);
                    Logger.Log("Kits have been refreshed", ConsoleColor.Yellow);
                } catch (Exception e)
                {
                    Logger.LogException(e);
                }                
            });
        }

        public void PostKit(Kit kit)
        {
            ThreadPool.QueueUserWorkItem((i) =>
            {
                try
                {
                    string contentString = JsonConvert.SerializeObject(kit);
                    using (WebClient wc = new WebClient())
                    {
                        wc.Headers.Add("x-api-key", Configuration.Instance.APIKey);
                        wc.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                        wc.UploadString(Configuration.Instance.APIUrl, contentString);
                    }                    
                    KitsCache.Add(kit);
                } catch (Exception e)
                {
                    Logger.LogException(e);
                }                
            });
        }

        protected override void Unload()
        {
            Logger.Log($"{Name} has been unloaded!", ConsoleColor.Yellow);
        }

        public override TranslationList DefaultTranslations => new TranslationList()
        {
            { "CreateKitFormat", "Format: /ckit <name> <cooldown> [experience]" },
            { "CreateKitExists", "The kit with such name already exists" },
            { "CreateKitInvalidCooldown", "Cooldown is in incorrect format" },
            { "CreateKitSuccess", "Successfully created kit {0} cooldown {1} with {2} items" },
            { "CooldownExpiredNotify", "Cooldown for kit {0} expired" },
            { "Kits", "Yours kits:" },
            { "NoKits", "You don't have access to any kits" },
            { "KitFormat", "Format: /kit <name>" },            
            { "KitNotFound", "Failed to find any kit with such name" },
            { "KitNoPermission", "You can't use this kit" },
            { "KitCooldown", "You have to wait {0} seconds before you can use this kit again" },
            { "KitSuccess", "You received kit {0}" },
            { "RefreshKitsPending", "Kits refresh request has been sent" }
        };
    }
}
