using System;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Rocket.Core;
using Rocket.Core.Logging;
using Rocket.Core.Plugins;
using WebPermissions.Provider;

namespace WebPermissions
{
    public class WebPermissionsPlugin : RocketPlugin<WebPermissionsConfiguration>
    {
        public IDatabase Database { get; set; }

        protected override void Load()
        {
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback((sender, certificate, chain, policyErrors) => { return true; });

            Database = new Database(this);
            R.Permissions = new SQLPermissionsProvider(Database);

            Logger.Log($"{Name} {Assembly.GetName().Version} has been loaded!", ConsoleColor.Yellow);
        }

        protected override void Unload()
        {
            Logger.Log($"{Name} has been unloaded!", ConsoleColor.Yellow);
        }
    }
}
