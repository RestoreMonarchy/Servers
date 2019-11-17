using Microsoft.AspNetCore.Authentication.Cookies;
using System.Threading.Tasks;

namespace Web.Server.Services
{
    public class CustomCookieAuthenticationEvents : CookieAuthenticationEvents
    {
        private readonly PlayersService _playersService;

        public CustomCookieAuthenticationEvents(PlayersService playersService)
        {
            _playersService = playersService;
        }

        public override async Task ValidatePrincipal(CookieValidatePrincipalContext context)
        {
            await _playersService.InitializePlayerAsync(context);
            await base.ValidatePrincipal(context);
        }
    }
}
