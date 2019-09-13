using Dapper;
using RestoreMonarchy.SQLPermissions.Models;
using RestoreMonarchy.SQLPermissions.Providers;
using Rocket.API;
using Rocket.API.Serialisation;
using Rocket.Core.Assets;
using Rocket.Core.Logging;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace RestoreMonarchy.SQLPermissions.Extensions
{
    public static class DatabaseExtension
    {
        public static RocketPermissionsProviderResult AddGroup(this IDatabaseManager database, RocketPermissionsGroup group)
        {
            string sql = "INSERT INTO dbo.PermissionGroups (GroupID, GroupName, GroupColor, GroupPriority) VALUES (@GroupID, @GroupName, @GroupColor, @GroupPriority);";
            int rows = 0;
            
            try
            {
                using (var conn = database.Connection)
                {   
                    rows = conn.Execute(sql, new { GroupID = group.Id, GroupName = group.DisplayName, GroupColor = group.Color, GroupPriority = group.Priority });
                }
            } catch (DbException e)
            {
                Logger.LogException(e);
            }

            return rows > 0 ? RocketPermissionsProviderResult.Success : RocketPermissionsProviderResult.DuplicateEntry;
        }

        public static RocketPermissionsProviderResult AddPlayerToGroup(this IDatabaseManager database, string groupId, ulong steamID)
        {
            string sql = "INSERT INTO dbo.PermissionMembers (GroupID, SteamID) VALUES (@GroupID, @SteamID);";
            int rows = 0;

            try
            {
                using (var conn = database.Connection)
                {
                    rows = conn.Execute(sql, new { GroupID = groupId, SteamID =  Convert.ToInt64(steamID) });
                }
            }
            catch (DbException e)
            {
                Logger.LogException(e);
            }

            return rows > 0 ? RocketPermissionsProviderResult.Success : RocketPermissionsProviderResult.GroupNotFound;
        }

        public static RocketPermissionsProviderResult DeleteGroup(this IDatabaseManager database, string groupId)
        {
            string sql = "DELETE FROM dbo.PermissionGroups WHERE GroupID = @GroupID;";
            int rows = 0;

            try
            {
                using (var conn = database.Connection)
                {
                    rows = conn.Execute(sql, new { GroupID = groupId });
                }
            }
            catch (DbException e)
            {
                Logger.LogException(e);
            }

            return rows > 0 ? RocketPermissionsProviderResult.Success : RocketPermissionsProviderResult.GroupNotFound;
        }

        public static RocketPermissionsGroup GetGroup(this IDatabaseManager database, string groupId)
        {
            string sql = "SELECT g.*, p.*, m.* FROM dbo.PermissionGroups AS g LEFT JOIN dbo.Permissions as p ON p.GroupID = g.GroupID " +
                "LEFT JOIN dbo.PermissionMemebers AS m ON m.GroupID = g.GroupID WHERE g.GroupID = @GroupID;";

            RocketPermissionsGroup group = null;

            try
            {
                using (var conn = database.Connection)
                {
                    conn.Query<PermissionGroup, PermissionGroup.Permission, PermissionGroup.PermissionMemeber, PermissionGroup>(sql, 
                        (g, p, m) => 
                        {
                            if (group == null)
                            {
                                group = new RocketPermissionsGroup(g.GroupID, g.GroupName, null, new List<string>(), new List<Permission>(), g.GroupColor, g.Priority);
                            }
                            group.Permissions.Add(new Permission(p.PermissionID));
                            group.Members.Add(m.SteamID.ToString());                            
                            return g;
                        }, splitOn: "GroupID,GroupID", param: new { GroupID = groupId });
                }
            }
            catch (DbException e)
            {
                Logger.LogException(e);
            }

            return group;
        }

        public static List<RocketPermissionsGroup> GetGroups(this IDatabaseManager database, ulong steamID)
        {
            string sql = "SELECT g.*, p.*, m.* FROM dbo.PermissionGroups AS g LEFT JOIN dbo.Permissions as p ON p.GroupID = g.GroupID " +
                "LEFT JOIN dbo.PermissionMemebers AS m ON m.GroupID = g.GroupID WHERE m.SteamID = @SteamID;";

            List<RocketPermissionsGroup> groups = new List<RocketPermissionsGroup>();
            try
            {
                using (var conn = database.Connection)
                {
                    conn.Query<PermissionGroup, PermissionGroup.Permission, PermissionGroup.PermissionMemeber, PermissionGroup>(sql,
                        (g, p, m) =>
                        {
                            RocketPermissionsGroup group = groups.FirstOrDefault(x => x.Id == g.GroupID);

                            if (group == null)
                            {
                                group = new RocketPermissionsGroup(g.GroupID, g.GroupName, null, new List<string>(), new List<Permission>(), g.GroupColor, g.Priority);
                                groups.Add(group);
                            }

                            if (p != null)
                                group.Permissions.Add(new Permission(p.PermissionID));
                            if (m != null)
                                group.Members.Add(m.SteamID.ToString());

                            return g;
                        }, splitOn: "GroupID,GroupID", param: new { SteamID = steamID });
                }
            }
            catch (DbException e)
            {
                Logger.LogException(e);
            }

            return groups;
        }

        public static RocketPermissionsProviderResult RemovePlayerFromGroup(this IDatabaseManager database, string groupId, ulong steamID)
        {
            string sql = "DELETE FROM dbo.PermissionMemebers WHERE GroupID = @GroupID AND SteamID = @SteamID;";
            int rows = 0;

            try
            {
                using (var conn = database.Connection)
                {
                    rows = conn.Execute(sql, new { GroupID = groupId, SteamID = steamID });
                }
            }
            catch (DbException e)
            {
                Logger.LogException(e);
            }

            return rows > 0 ? RocketPermissionsProviderResult.Success : RocketPermissionsProviderResult.GroupNotFound;
        }
    }
}
