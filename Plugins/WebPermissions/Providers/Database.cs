using RestoreMonarchy.Core;
using Rocket.API;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;
using System;

namespace RestoreMonarchy.WebPermissions.Providers
{
    public class Database : IDatabase
    {
        private readonly WebPermissionsPlugin pluginInstance;
        private WebClient webClient { get; set; }
        private string apiUrl => pluginInstance.Configuration.Instance.APIUrl.TrimEnd('/');

        public Database(WebPermissionsPlugin pluginInstance)
        {
            this.pluginInstance = pluginInstance;
            webClient = new WebClient();
            webClient.Headers.Add("x-api-key", pluginInstance.Configuration.Instance.WebAPIKey);            
        }

        public RocketPermissionsProviderResult AddPlayerToGroup(string groupID, ulong steamID)
        {
            webClient.Headers.Add(HttpRequestHeader.ContentType, "application/json");
            PermissionGroup.PermissionMemeber member = new PermissionGroup.PermissionMemeber();
            string dataString = JsonConvert.SerializeObject(member);
            int rows = Convert.ToInt32(webClient.UploadString("api/Permissions/Members", dataString));
            return rows > 0 ? RocketPermissionsProviderResult.Success : RocketPermissionsProviderResult.DuplicateEntry;
        }

        public PermissionGroup GetGroup(string groupID)
        {
            string dataString = webClient.DownloadString(apiUrl + "/Groups/" + groupID);            
            return JsonConvert.DeserializeObject<PermissionGroup>(dataString);
        }

        public List<PermissionGroup> GetGroups(ulong steamID)
        {
            webClient.Headers.Add(HttpRequestHeader.ContentType, "application/x-www-form-urlencoded");
            string dataString = webClient.DownloadString(apiUrl + "/Members/" + steamID);
            return JsonConvert.DeserializeObject<List<PermissionGroup>>(dataString);
        }

        public RocketPermissionsProviderResult RemovePlayerFromGroup(string groupID, ulong steamID)
        {
            throw new System.NotImplementedException();
        }
    }
}
