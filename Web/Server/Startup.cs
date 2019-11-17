using AspNet.Security.ApiKey.Providers.Events;
using AspNet.Security.ApiKey.Providers.Extensions;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RestoreMonarchy.Database;
using SteamWebAPI2.Utilities;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Web.Server.Utilities.DiscordMessager;
using Microsoft.Extensions.Logging;
using Web.Server.Services;

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
            services.AddLogging();
            services.AddSingleton(new SteamWebInterfaceFactory(Configuration["SteamAPI"]));
            services.AddSingleton<IDatabaseManager>(new DatabaseManager(Configuration.GetConnectionString("DefaultConnection"), Configuration.GetValue<string>(WebHostDefaults.ContentRootKey)));
            services.AddTransient<HttpClient>();
            services.AddSingleton<DiscordMessager>();
            services.AddSingleton<PlayersService>();
            services.AddSingleton<CustomCookieAuthenticationEvents>();
            
            
            services.AddAuthentication(options => { options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;})
                .AddCookie(options =>
                {
                    options.LoginPath = "/signin";
                    options.LogoutPath = "/signout";                    
                    options.EventsType = typeof(CustomCookieAuthenticationEvents);
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

            // for Nginx
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.KnownProxies.Add(System.Net.IPAddress.Parse("10.0.0.100"));
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddFile("Logs/web.server-{Date}.log");
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
