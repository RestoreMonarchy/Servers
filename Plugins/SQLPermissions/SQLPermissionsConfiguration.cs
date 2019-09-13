using Rocket.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestoreMonarchy.SQLPermissions
{
    public class SQLPermissionsConfiguration : IRocketPluginConfiguration
    {
        public string ConnectionString { get; set; }

        public void LoadDefaults()
        {
            ConnectionString = "Server=localhost\\sqlexpress;Database=Unturned;User Id=sa;Password=ZombieZone!123;";  
        }
    }
}
