using Dapper;
using MySql.Data.MySqlClient;
using Rocket.Core.Logging;
using System;

namespace RestoreMonarchy.Moderation.Database
{
    public class DatabaseManager
    {
        private readonly ModerationPlugin pluginInstance;

        public DatabaseManager(ModerationPlugin pluginInstance)
        {
            this.pluginInstance = pluginInstance;            

            if (Connection == null)
                pluginInstance.UnloadPlugin(Rocket.API.PluginState.Failure);

            try
            {
                InitializeTables();
            } catch (Exception e)
            {
                Console.WriteLine(e);
            }
            
        }

        public void InitializeTables()
        {            
            using (var conn = Connection)
            {
                try
                {
                    conn.Execute(DatabaseQuery.PlayersTable);
                    conn.Execute(DatabaseQuery.BansTable);
                }
                catch (MySqlException e)
                {
                    Logger.LogError("An error occurated while trying to create tables.");
                    Logger.LogException(e);
                    pluginInstance.UnloadPlugin(Rocket.API.PluginState.Failure);
                }
            }
        }

        public MySqlConnection Connection
        {
            get
            {
                MySqlConnection conn;
                try
                {
                    conn = new MySqlConnection(pluginInstance.Configuration.Instance.ConnectionString);
                }
                catch (MySqlException e)
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
