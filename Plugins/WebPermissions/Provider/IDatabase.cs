using System.Collections.Generic;
using Core;
using Rocket.API;

namespace WebPermissions.Provider
{
    public interface IDatabase
    {
        RocketPermissionsProviderResult AddPlayerToGroup(string groupId, ulong steamId);
        PermissionGroup GetGroup(string groupId);
        List<PermissionGroup> GetGroups(ulong steamId);
        RocketPermissionsProviderResult RemovePlayerFromGroup(string groupId, ulong steamId);
    }
}
