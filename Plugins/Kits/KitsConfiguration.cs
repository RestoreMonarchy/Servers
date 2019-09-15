using Core;
using Rocket.API;
using System.Collections.Generic;

namespace Kits
{
    public class KitsConfiguration : IRocketPluginConfiguration
    {
        public List<Kit> Kits { get; set; }

        public void LoadDefaults()
        {
            Kits = new List<Kit>()
            {
                new Kit()
            };
        }
    }
}