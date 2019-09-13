using Rocket.Core.Logging;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestoreMonarchy.SQLPermissions.Providers
{
    public class DatabaseManager : IDatabaseManager
    {
        private readonly SQLPermissionsPlugin pluginInstance;

        public DatabaseManager(SQLPermissionsPlugin pluginInstance)
        {
            this.pluginInstance = pluginInstance;

            if (Connection == null)
                pluginInstance.UnloadPlugin(Rocket.API.PluginState.Failure);
        }

        public DbConnection Connection
        {
            get
            {
                SqlConnection conn;
                try
                {
                    conn = new SqlConnection(pluginInstance.Configuration.Instance.ConnectionString);
                }
                catch (SqlException e)
                {
                    Logger.LogWarning("Failed to connect to database. Check your connection string!");
                    Logger.LogException(e);
                    conn = null;
                }
                return conn;
            }
        }


    }
}
