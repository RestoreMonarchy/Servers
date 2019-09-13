using Rocket.API;
using Rocket.API.Serialisation;
using System;
using System.Collections.Generic;
using RestoreMonarchy.SQLPermissions.Extensions;
using System.Linq;
using Rocket.Core.Logging;

namespace RestoreMonarchy.SQLPermissions.Providers
{
    public class SQLPermissionsProvider : IRocketPermissionsProvider
    {
        public SQLPermissionsProvider(IDatabaseManager database)
        {
            this.database = database;
        }
        private IDatabaseManager database;

        public RocketPermissionsProviderResult AddGroup(RocketPermissionsGroup group)
        {
            return database.AddGroup(group);
        }

        public RocketPermissionsProviderResult AddPlayerToGroup(string groupId, IRocketPlayer player)
        {
            if (ulong.TryParse(player.Id, out ulong steamID))
                return database.AddPlayerToGroup(groupId, steamID);
            else
                return RocketPermissionsProviderResult.PlayerNotFound;
        }

        public RocketPermissionsProviderResult DeleteGroup(string groupId)
        {
            return database.DeleteGroup(groupId);
        }

        public RocketPermissionsGroup GetGroup(string groupId)
        {
            return database.GetGroup(groupId);
        }

        public List<RocketPermissionsGroup> GetGroups(IRocketPlayer player, bool includeParentGroups)
        {
            if (ulong.TryParse(player.Id, out ulong steamID))
                return database.GetGroups(steamID);
            else
                return null;
        }

        public List<Permission> GetPermissions(IRocketPlayer player)
        {
            List<RocketPermissionsGroup> groups = GetGroups(player, false);
            List<Permission> permissions = new List<Permission>();

            if (groups != null)
            {
                foreach (RocketPermissionsGroup group in groups)
                {
                    foreach (Permission permission in group.Permissions)
                    {
                        permissions.Add(permission);
                    }
                }
            }

            return permissions;            
        }

        public List<Permission> GetPermissions(IRocketPlayer player, List<string> requestedPermissions)
        {
            return (from p in GetPermissions(player)
                           where requestedPermissions.Exists((string i) => string.Equals(i, p.Name, StringComparison.OrdinalIgnoreCase))
                           select p).ToList();
        }

        public bool HasPermission(IRocketPlayer player, List<string> requestedPermissions)
        {
            if (player.IsAdmin)
                return true;

            List<Permission> perms = GetPermissions(player);
            List<string> permsAsStrs = new List<string>();

            foreach (Permission perm in perms)
                permsAsStrs.Add(perm.Name);

            foreach (string str in requestedPermissions)
            {
                if (permsAsStrs.Contains(str) && !permsAsStrs.Contains("~" + str))
                    return true;
            }

            return false;
        }

        public void Reload()
        {
            Logger.LogWarning("Reloading permissions is not implemented");
        }

        public RocketPermissionsProviderResult RemovePlayerFromGroup(string groupId, IRocketPlayer player)
        {
            if (ulong.TryParse(player.Id, out ulong steamID))
                return database.RemovePlayerFromGroup(groupId, steamID);
            else
                return RocketPermissionsProviderResult.PlayerNotFound;
        }

        public RocketPermissionsProviderResult SaveGroup(RocketPermissionsGroup group)
        {
            return RocketPermissionsProviderResult.UnspecifiedError;
        }
    }
}
