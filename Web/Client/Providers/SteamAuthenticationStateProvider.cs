using Core.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Web.Client.Providers
{
    public class SteamAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly HttpClient _httpClient;

        public SteamAuthenticationStateProvider(HttpClient httpClient)
        {
            this._httpClient = httpClient;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var userInfo = await _httpClient.GetJsonAsync<UserInfo>("user");

            var steamIdentity = userInfo.IsAuthenticated
                ? new ClaimsIdentity(new[] { 
                    new Claim(ClaimTypes.Name, userInfo.Player.PlayerId.ToString()), 
                    new Claim(ClaimTypes.GivenName, userInfo.Player.PlayerName), 
                    new Claim(ClaimTypes.Role, userInfo.Player.Role), 
                    new Claim(ClaimTypes.Country, userInfo.Player.PlayerCountry), 
                    new Claim("Balance", userInfo.Player.Balance.ToString("{0:0.##}"))}, "steamauth")
                : new ClaimsIdentity();

            return new AuthenticationState(new ClaimsPrincipal(steamIdentity));
        }

        public void NotifyAuthenticationStateChanged()
        {
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }
    }
}
