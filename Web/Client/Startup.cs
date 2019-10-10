using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Builder;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Net.Http;
using Web.Client.Providers;

namespace Web.Client
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthorizationCore();
            services.AddScoped<AuthenticationStateProvider, SteamAuthenticationStateProvider>();
        }

        public void Configure(IComponentsApplicationBuilder app)
        {
            app.AddComponent<App>("app");
        }
    }
}
