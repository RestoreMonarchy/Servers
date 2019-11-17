using Core.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using RestoreMonarchy.Database;
using SteamWebAPI2.Interfaces;
using SteamWebAPI2.Utilities;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Web.Server.Extensions.Database;

namespace Web.Server.Services
{
    public class PlayersService
    {
        private readonly IDatabaseManager _database;
        private readonly SteamWebInterfaceFactory _steamFactory;
        private readonly ILogger<PlayersService> _logger;

        public PlayersService(IDatabaseManager database, SteamWebInterfaceFactory steamFactory, ILogger<PlayersService> logger)
        {
            _database = database;
            _steamFactory = steamFactory;
            _logger = logger;
        }

        public async Task<Player> GetInitializedPlayerAsync(string steamId, string ip)
        {
            Player player = _database.GetPlayer(steamId);
            if (player == null)
            {
                var steamUser = _steamFactory.CreateSteamWebInterface<SteamUser>(new HttpClient());
                // pretty risky here using ulong.Parse()
                var summaries = await steamUser.GetPlayerSummaryAsync(ulong.Parse(steamId));
                player = new Player(steamId, summaries.Data.Nickname, await GetCountryAsync(ip));

                _database.CreatePlayer(player);
            } else
            {
                _database.UpdateLastActivity(steamId);
            }
            return player;
        }

        public async Task InitializePlayerAsync(CookieValidatePrincipalContext context)
        {
            string steamId = context.Principal.FindFirst(ClaimTypes.NameIdentifier).Value.Substring(37);
            string ip = context.Request.HttpContext.Connection.RemoteIpAddress.ToString();

            Player player = await GetInitializedPlayerAsync(steamId, ip);

            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Name, player.PlayerId));
            claims.Add(new Claim(ClaimTypes.Role, player.Role));

            context.Principal.AddIdentity(new ClaimsIdentity(claims, "DefaultAuth"));
        }

        public async Task<string> GetCountryAsync(string ip)
        {
            HttpClient httpClient = new HttpClient();
            httpClient.Timeout = TimeSpan.FromSeconds(2);
            string countryCode = null;
            try
            {
                string url = "http://ip-api.com/json/" + ip;
                string content = await httpClient.GetStringAsync(url);
                JObject obj = JObject.Parse(content);

                if (obj["status"].ToString() == "success")
                {
                    countryCode = obj["countryCode"].ToString();
                }
            } catch (Exception e)
            {
                _logger.LogError(e, "There was an error while trying to get {0} country!", ip);
            }
            return countryCode;
        }
    }
}
