using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Web.Server.Utilities
{
    public class AvatarManager
    {
        private readonly IConfiguration configuration;
        private readonly string directory;
        private readonly string avatarDir;

        [Obsolete]
        public AvatarManager(IHostingEnvironment environment, IConfiguration configuration)
        {
            this.configuration = configuration;
            this.directory = environment.ContentRootPath + "/wwwroot/img/";
            this.avatarDir = directory + "avatars/";
        }

        public void SaveSteamPlayerAvatar(ulong steamId)
        {
            string url = "http://" + "api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key=" + configuration["SteamAPI"] + "&steamids=" + steamId;
            string data;
            using (WebClient wc = new WebClient())
            {
                data = wc.DownloadString(url);
            }

            string avatarUrl = JObject.Parse(data)["response"]["players"][0]["avatarfull"].ToString();
            SaveAvatar(avatarUrl, steamId);
        }

        public void SaveAvatar(string url, ulong steamId)
        {
            using (WebClient wc = new WebClient())
            {
                wc.DownloadFile(url, @"D:\MCrow\Github\RestoreMonarchy\Web\Client\wwwroot\img\avatars\" + steamId + ".jpg");
            }
        }
    }
}
