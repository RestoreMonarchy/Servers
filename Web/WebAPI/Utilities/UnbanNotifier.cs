using RestoreMonarchy.WebAPI.Models;

namespace RestoreMonarchy.WebAPI.Utilities
{
    public class UnbanNotifier
    {
        private readonly Database database;
        private readonly DiscordMessager messager;
        public UnbanNotifier(Database database, DiscordMessager messager)
        {
            this.database = database;
            this.messager = messager;
            Initialize();
        }

        private void Initialize()
        {
            
        }
    }
}