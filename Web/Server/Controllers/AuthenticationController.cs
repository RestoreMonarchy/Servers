using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using System.Linq;
using Core.Models;
using Web.Server.Utilities;
using Dapper;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Web.Server.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly Database database;
        private readonly HttpClient httpClient;
        private readonly SteamUtility steam;

        private SqlConnection connection => new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

        public AuthenticationController(IConfiguration configuration, Database database, HttpClient httpClient, SteamUtility steam)
        {
            this.configuration = configuration;
            this.database = database;
            this.httpClient = httpClient;
            this.steam = steam;
        }

        [HttpGet("~/user")]
        public UserInfo GetUser()
        {   
            if (User.Identity.IsAuthenticated)
            {                
                var player = database.GetPlayer(User.Claims.First().Value.Substring(37));
                return new UserInfo { Player = player, IsAuthenticated = true };
            } else
            {
                return new UserInfo() { IsAuthenticated = false };
            }
        }

        [HttpGet("~/inituser")]
        public async Task<Player> InitializeUserAsync()
        {
            if (!User.Identity.IsAuthenticated)
                return null;

            string url = "http://ip-api.com/json/" + Request.HttpContext.Connection.RemoteIpAddress.ToString();
            System.Console.WriteLine(Request.HttpContext.Connection.RemoteIpAddress.ToString());
            string content = await httpClient.GetStringAsync(url);
            JObject obj = JObject.Parse(content);

            string countryCode = null;
            if (obj["status"].ToString() == "success")
            {
                countryCode = obj["countryCode"].ToString();
            }

            string steamId = User.Claims.First().Value.Substring(37);
            var steamPlayer = await steam.GetSteamPlayerAsync(steamId);

            Player player = null;

            if (steamPlayer != null)
            {
                player = new Player(steamId, steamPlayer.personaname, countryCode);
                player = database.CreatePlayer(player);
            }

            return player;
        }

        [HttpPost("~/signin")]
        public IActionResult SignIn(string returnUrl = "/")
        {
            return Challenge(new AuthenticationProperties { RedirectUri = returnUrl }, "Steam");
        }

        [HttpGet("~/signout"), HttpPost("~/signout")]
        public IActionResult SignOut()
        {
            return SignOut(new AuthenticationProperties { RedirectUri = "/" },
                CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
}