using System;
using System.Collections.Generic;
using System.Net;
using Core;
using Newtonsoft.Json;
using Rocket.API;

namespace WebPermissions.Provider
{
    public class Database : IDatabase
    {
        private readonly WebPermissionsPlugin _pluginInstance;
        private WebClient WebClient { get; set; }
        private string ApiUrl => _pluginInstance.Configuration.Instance.ApiUrl;

        public Database(WebPermissionsPlugin pluginInstance)
        {
            this._pluginInstance = pluginInstance;
            WebClient = new WebClient();
            WebClient.Headers.Add("x-api-key", pluginInstance.Configuration.Instance.WebApiKey);            
        }

        public RocketPermissionsProviderResult AddPlayerToGroup(string groupId, ulong steamId)
        {
            WebClient.Headers.Add(HttpRequestHeader.ContentType, "application/json");
            PermissionGroup.PermissionMember member = new PermissionGroup.PermissionMember(groupId, steamId);
            string dataString = JsonConvert.SerializeObject(member);
            int rows = Convert.ToInt32(WebClient.UploadString(ApiUrl + "/Members", dataString));
            return rows > 0 ? RocketPermissionsProviderResult.Success : RocketPermissionsProviderResult.DuplicateEntry;
        }

        public PermissionGroup GetGroup(string groupId)
        {
            string dataString = WebClient.DownloadString(ApiUrl + "/Groups/" + groupId);            
            return JsonConvert.DeserializeObject<PermissionGroup>(dataString);
        }

        public List<PermissionGroup> GetGroups(ulong steamId)
        {
            string dataString = WebClient.DownloadString(ApiUrl + "/Members/" + steamId);
            return JsonConvert.DeserializeObject<List<PermissionGroup>>(dataString);
        }

        public RocketPermissionsProviderResult RemovePlayerFromGroup(string groupId, ulong steamId)
        {
            WebClient.Headers.Add(HttpRequestHeader.ContentType, "application/json");
            PermissionGroup.PermissionMember member = new PermissionGroup.PermissionMember(groupId, steamId);
            string dataString = JsonConvert.SerializeObject(member);
            int rows = Convert.ToInt32(WebClient.UploadString(ApiUrl + "/Members", "DELETE", dataString));
            return rows > 0 ? RocketPermissionsProviderResult.Success : RocketPermissionsProviderResult.DuplicateEntry;
        }
    }
}
