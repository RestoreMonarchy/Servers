using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Rocket.API;
using Rocket.API.Serialisation;

namespace WebPermissions.Provider
{
    public class SQLPermissionsProvider : IRocketPermissionsProvider
    {
        public SQLPermissionsProvider(IDatabase database)
        {
            this.database = database;
        }

        private IDatabase database;

        public RocketPermissionsProviderResult AddPlayerToGroup(string groupId, IRocketPlayer player)
        {
            if (ulong.TryParse(player.Id, out ulong steamID))
                return database.AddPlayerToGroup(groupId, steamID);
            else
                return RocketPermissionsProviderResult.PlayerNotFound;
        }

        public RocketPermissionsGroup GetGroup(string groupId)
        {
            PermissionGroup group = database.GetGroup(groupId);
            RocketPermissionsGroup rocketGroup = new RocketPermissionsGroup()
            {
                Id = group.GroupID,
                DisplayName = group.GroupName,
                Members = group.Members.Select(x => x.SteamID.ToString()).ToList(),
                Permissions = group.Permissions.Select(x => new Permission(x.PermissionID)).ToList(),
                Color = group.GroupColor,
                Priority = group.GroupPriority
            };

            return rocketGroup;
        }

        public List<RocketPermissionsGroup> GetGroups(IRocketPlayer player, bool includeParentGroups)
        {
            if (ulong.TryParse(player.Id, out ulong steamID))
            {
                List<RocketPermissionsGroup> rocketGroups = new List<RocketPermissionsGroup>();
                List<PermissionGroup> groups = database.GetGroups(steamID);

                if (groups != null)
                {
                    foreach (PermissionGroup group in groups)
                    {
                        rocketGroups.Add(new RocketPermissionsGroup(group.GroupID, group.GroupName, null, group.Members.Select(x => x.SteamID.ToString()).ToList(), 
                            group.Permissions.Select(x => new Permission(x.PermissionID)).ToList(), group.GroupColor, group.GroupPriority));
                    }
                }

                return rocketGroups;
            }                
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

        public RocketPermissionsProviderResult RemovePlayerFromGroup(string groupId, IRocketPlayer player)
        {
            if (ulong.TryParse(player.Id, out ulong steamID))
                return database.RemovePlayerFromGroup(groupId, steamID);
            else
                return RocketPermissionsProviderResult.PlayerNotFound;
        }

        public void Reload()
        {
        }
        public RocketPermissionsProviderResult AddGroup(RocketPermissionsGroup group)
        {
            return RocketPermissionsProviderResult.UnspecifiedError;
        }
        public RocketPermissionsProviderResult DeleteGroup(string groupId)
        {
            return RocketPermissionsProviderResult.UnspecifiedError;
        }
        public RocketPermissionsProviderResult SaveGroup(RocketPermissionsGroup group)
        {
            return RocketPermissionsProviderResult.UnspecifiedError;
        }
    }
}
