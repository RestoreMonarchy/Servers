using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Core.Models;
using Web.Server.Utilities;

namespace Web.Server.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly Database database;

        private SqlConnection connection => new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

        public AuthenticationController(IConfiguration configuration, Database database)
        {
            this.configuration = configuration;
            this.database = database;
        }

        [HttpGet("~/user")]
        public UserInfo GetUser()
        {
            if (User.Identity.IsAuthenticated && ulong.TryParse(User.Claims.First().Value.Substring(37), out ulong steamId))
            {
                return new UserInfo { Name = database.GetPlayer(steamId).PlayerName, IsAuthenticated = true, SteamId = steamId };
            } else
            {
                return new UserInfo() { IsAuthenticated = false };
            }
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
