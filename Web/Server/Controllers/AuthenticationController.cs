﻿using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Core.Models;
using Web.Server.Extensions.Database;
using System.Security.Claims;
using RestoreMonarchy.Database;

namespace Web.Server.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly IDatabaseManager _database;
        
        public AuthenticationController(IDatabaseManager database)
        {
            _database = database;
        }

        [HttpGet("~/user")]
        public UserInfo GetUser()
        {   
            if (User.Identity.IsAuthenticated)
            {                
                var player = _database.GetPlayer(User.FindFirst(ClaimTypes.Name).Value);
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