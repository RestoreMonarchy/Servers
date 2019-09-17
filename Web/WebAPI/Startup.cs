using AspNet.Security.ApiKey.Providers;
using AspNet.Security.ApiKey.Providers.Events;
using AspNet.Security.ApiKey.Providers.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RestoreMonarchy.WebAPI.Models;
using RestoreMonarchy.WebAPI.Utilities;
using System.Diagnostics;
using System.Threading.Tasks;

namespace RestoreMonarchy.WebAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddAuthentication(options =>
            //{
            //    options.DefaultAuthenticateScheme = ApiKeyDefaults.AuthenticationScheme;
            //    options.DefaultChallengeScheme = ApiKeyDefaults.AuthenticationScheme;
            //})
            //.AddApiKey(options =>
            //{
            //    options.Events = new ApiKeyEvents
            //    {
            //        OnAuthenticationFailed = context =>
            //        {
            //            Trace.TraceError(context.Exception.Message);

            //            return Task.CompletedTask;
            //        },
            //        OnApiKeyValidated = context =>
            //        {
            //            if (context.ApiKey == "123")
            //            {
            //                context.Success();
            //            }

            //            return Task.CompletedTask;
            //        }
            //    };
            //    options.Header = "x-api-key";
            //    options.HeaderKey = string.Empty;
            //});

            services.AddSingleton<UnbanNotifier>();
            services.AddSingleton<Database>();
            services.AddSingleton<DiscordMessager>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseMvc(routes =>
            {
                routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
