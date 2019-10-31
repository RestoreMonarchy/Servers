using AspNet.Security.ApiKey.Providers;
using AspNet.Security.ApiKey.Providers.Events;
using AspNet.Security.ApiKey.Providers.Extensions;
using Core.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Web.Server.Utilities;
using Web.Server.Utilities.Database;
using Web.Server.Utilities.DiscordMessager;

namespace Web.Server
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
            DatabaseManager database = new DatabaseManager(Configuration);
            services.AddTransient<HttpClient>();
            services.AddSingleton<DiscordMessager>();
            services.AddSingleton(database);
            
            services.AddAuthentication(options => { options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;})
                .AddCookie(options =>
                {
                    options.LoginPath = "/signin";
                    options.LogoutPath = "/signout";
                    options.Events.OnValidatePrincipal = database.InitializePlayerAsync;
                }).AddSteam().AddApiKey(options =>
                {
                    options.Events = new ApiKeyEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            Trace.TraceError(context.Exception.Message);
                            return Task.CompletedTask;
                        },
                        OnApiKeyValidated = context =>
                        {
                            if (context.ApiKey == Configuration["ApiKey"])
                            {
                                context.Success();
                            }

                            return Task.CompletedTask;
                        }
                    };
                    options.Header = "x-api-key";
                    options.HeaderKey = string.Empty;
                }); 


            services.AddMvc().AddNewtonsoftJson();
            services.AddResponseCompression(opts =>
            {
                opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                    new[] { "application/octet-stream" });
            });

            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.KnownProxies.Add(System.Net.IPAddress.Parse("10.0.0.100"));
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseStaticFiles();
            app.UseClientSideBlazorFiles<Client.Startup>();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                endpoints.MapFallbackToClientSideBlazor<Client.Startup>("index.html");
            });
        }
    }
}
