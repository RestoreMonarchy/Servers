using Core.Models;
using Rocket.API;
using System.Collections.Generic;

namespace Kits
{
    public class KitsConfiguration : IRocketPluginConfiguration
    {
        public string MessageColor { get; set; }
        public string APIUrl { get; set; }
        public string APIKey { get; set; }

        public void LoadDefaults()
        {
            MessageColor = "yellow";
            APIUrl = "https://servers.restoremonarchy.com/api/kits";
            APIKey = "API_KEY";
        }
    }
}