using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestoreMonarchy.Moderation.Database
{
    public interface IDatabase
    {
        Player GetPlayer(ulong steamId);
        Player CreatePlayer(this DatabaseManager manager, Player player)
    }
}
