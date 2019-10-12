using Core.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
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

            ClaimsIdentity steamIdentity;

            if (userInfo.IsAuthenticated)
            {
                steamIdentity = GetClaimsIdentity(userInfo.Player);
            } else
            {
                steamIdentity = new ClaimsIdentity();
            }

            return new AuthenticationState(new ClaimsPrincipal(steamIdentity));
        }

        private ClaimsIdentity GetClaimsIdentity(Player player)
        {
            return new ClaimsIdentity(new[] {
                        new Claim(ClaimTypes.Name, player.PlayerId),
                        new Claim(ClaimTypes.GivenName, player.PlayerName),                        
                        new Claim(ClaimTypes.Role, player.Role),
                        new Claim(ClaimTypes.Country, player.PlayerCountry == null ? string.Empty : player.PlayerCountry),
                        new Claim("Avatar", Convert.ToBase64String(player.PlayerAvatar)),
                        new Claim("Balance", player.Balance.ToString("{0:0.##}"))}, "steamauth");
        }

        public void NotifyAuthenticationStateChanged()
        {
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }
    }
}
