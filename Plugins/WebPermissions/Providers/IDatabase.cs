using RestoreMonarchy.Core;
using Rocket.API;
using Rocket.API.Serialisation;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestoreMonarchy.WebPermissions.Providers
{
    public interface IDatabase
    {
        RocketPermissionsProviderResult AddPlayerToGroup(string groupID, ulong steamID);
        PermissionGroup GetGroup(string groupID);
        List<PermissionGroup> GetGroups(ulong steamID);
        RocketPermissionsProviderResult RemovePlayerFromGroup(string groupID, ulong steamID);
    }
}
