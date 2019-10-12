using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Core.Models;
using Web.Server.Utilities;

namespace Web.Server.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly Database database;
        
        public AuthenticationController(Database database)
        {
            this.database = database;
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