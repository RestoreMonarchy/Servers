using Core.Models;
using RestoreMonarchy.Audit.Utilities;
using Rocket.API;
using Rocket.API.Serialisation;
using Rocket.Core.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using Timer = System.Timers.Timer;

namespace RestoreMonarchy.Audit.Providers
{
    public class PermissionsProvider : IRocketPermissionsProvider
    {
        private readonly AuditPlugin pluginInstance;
        private readonly Timer timer;
        public List<Rank> PlayerRanks { get; set; }
        private List<RocketPermissionsGroup> permissionGroups { get; set; }

        public PermissionsProvider(AuditPlugin plugin)
        {
            pluginInstance = plugin;
            timer = new Timer(plugin.Configuration.Instance.RefreshInterval);
            timer.Elapsed += (o, e) => RefreshPermissions();            
            RefreshPermissions();
            timer.Start();
        }

        public void RefreshPermissions()
        {            
            ThreadPool.QueueUserWorkItem((i) => 
            {
                Logger.LogWarning("Refreshing permissions...");
                using (WebClient wc = pluginInstance.Client)
                {
                    PlayerRanks = wc.DownloadJson<List<Rank>>(pluginInstance.Configuration.Instance.APIUrl + "/ranks/server");
                }

                Logger.LogWarning($"{PlayerRanks.Count} player ranks have been loaded!");
                List<RocketPermissionsGroup> groups = new List<RocketPermissionsGroup>();
                foreach (var rank in PlayerRanks)
                {
                    var perms = rank.PermissionTags.Split(',').Select(p => new Permission(p)).ToList();
                    groups.Add(new RocketPermissionsGroup(rank.RankId.ToString(), rank.Name, string.Empty, rank.Members, perms));
                }
                permissionGroups = groups;

                Logger.LogWarning($"{groups.Count} groups have been loaded!");

            });
        }
        

        public RocketPermissionsProviderResult AddGroup(RocketPermissionsGroup group)
        {
            Logger.LogWarning("AddGroup is not supported by PermissionsProvider");
            return RocketPermissionsProviderResult.UnspecifiedError;
        }

        public RocketPermissionsProviderResult AddPlayerToGroup(string groupId, IRocketPlayer player)
        {
            Logger.LogWarning("AddPlayerToGroup is not supported by PermissionsProvider");
            return RocketPermissionsProviderResult.UnspecifiedError;
        }

        public RocketPermissionsProviderResult DeleteGroup(string groupId)
        {
            Logger.LogWarning("DeleteGroup is not supported by PermissionsProvider");
            return RocketPermissionsProviderResult.UnspecifiedError;
        }

        public RocketPermissionsGroup GetGroup(string groupId)
        {
            return permissionGroups.FirstOrDefault(x => x.DisplayName.Equals(groupId, StringComparison.OrdinalIgnoreCase) || x.Id.ToString() == groupId);            
        }

        public List<RocketPermissionsGroup> GetGroups(IRocketPlayer player, bool includeParentGroups = false)
        {
            return permissionGroups.Where(x => x.Members.Exists(y => y == player.Id)).ToList();
        }

        public List<Permission> GetPermissions(IRocketPlayer player)
        {
            var groups = GetGroups(player);
            List<Permission> permissions = new List<Permission>();
            foreach (var group in groups)
            {
                group.Permissions.ForEach((item) => permissions.Add(item));
            }
            return permissions;
        }

        public List<Permission> GetPermissions(IRocketPlayer player, List<string> requestedPermissions)
        {
            return GetPermissions(player).Where(x => requestedPermissions.Exists(y => y.Equals(x.Name, StringComparison.OrdinalIgnoreCase))).GroupBy(x=> x.Name).Select(x => x.First()).ToList();
        }

        public bool HasPermission(IRocketPlayer player, List<string> requestedPermissions)
        {
            return player.IsAdmin ? true : GetPermissions(player, requestedPermissions).Count == requestedPermissions.Count ? true : false;
        }

        public void Reload()
        {
            RefreshPermissions();
        }

        public RocketPermissionsProviderResult RemovePlayerFromGroup(string groupId, IRocketPlayer player)
        {
            Logger.LogWarning("RemovePlayerFromGroup is not supported by PermissionsProvider");
            return RocketPermissionsProviderResult.UnspecifiedError;
        }

        public RocketPermissionsProviderResult SaveGroup(RocketPermissionsGroup group)
        {
            Logger.LogWarning("SaveGroup is not supported by PermissionsProvider");
            return RocketPermissionsProviderResult.UnspecifiedError;
        }
    }
}
