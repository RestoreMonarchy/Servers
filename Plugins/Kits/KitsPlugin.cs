using Rocket.API.Collections;
using Rocket.Core.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kits
{
    public class KitsPlugin : RocketPlugin<KitsConfiguration>
    {
        protected override void Load()
        {

        }

        public override TranslationList DefaultTranslations => new TranslationList()
        {
            { "", "" },
            { "", "" }
        };
    }
}
