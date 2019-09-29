using Blazorise;
using Blazorise.Icons.Material;
using Blazorise.Material;
using Microsoft.AspNetCore.Components.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;

namespace Web.Client
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddBlazorise(options =>
                {
                    options.ChangeTextOnKeyPress = true;
                })
                .AddMaterialProviders()
                .AddMaterialIcons();
        }

        public void Configure(IComponentsApplicationBuilder app)
        {
            app.Services
                .UseMaterialProviders()
                .UseMaterialIcons();

            app.AddComponent<App>("app");
        }
    }
}
