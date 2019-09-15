using Core;
using Rocket.API;
using System.Collections.Generic;

namespace Kits
{
    public class KitsConfiguration : IRocketPluginConfiguration
    {
        public string MessageColor { get; set; }
        public List<Kit> Kits { get; set; }

        public void LoadDefaults()
        {
            MessageColor = "#10ffdb";
            Kits = new List<Kit>()
            {
                new Kit()
            };
        }
    }
}