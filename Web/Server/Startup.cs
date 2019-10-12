using Core.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Web.Server.Utilities;
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
        private Database database;
        private SteamUtility steam;

        public void ConfigureServices(IServiceCollection services)
        {
            database = new Database(Configuration);
            steam = new SteamUtility(new HttpClient(), Configuration);
            services.AddSingleton<DiscordMessager>();
            services.AddSingleton(steam);
            services.AddSingleton(database);
            services.AddTransient<HttpClient>();
            
            services.AddAuthentication(options => { options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme; })
                .AddCookie(options =>
                {
                    options.LoginPath = "/signin";
                    options.LogoutPath = "/signout";
                    options.Events.OnValidatePrincipal = IntializeUserAsync;
                }).AddSteam();

            services.AddMvc().AddNewtonsoftJson();
            services.AddResponseCompression(opts =>
            {
                opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                    new[] { "application/octet-stream" });
            });
        }

        private async Task IntializeUserAsync(CookieValidatePrincipalContext context)
        {
            string steamId = context.Principal.FindFirst(ClaimTypes.NameIdentifier).Value.Substring(37);
            Player player = database.GetPlayer(steamId);

            if (player == null)
            {
                HttpClient httpClient = new HttpClient();
                string url = "http://ip-api.com/json/" + context.Request.HttpContext.Connection.RemoteIpAddress.ToString();
                string content = await httpClient.GetStringAsync(url);
                JObject obj = JObject.Parse(content);

                string countryCode = null;
                if (obj["status"].ToString() == "success")
                {
                    countryCode = obj["countryCode"].ToString();
                }

                var steamPlayer = await steam.GetSteamPlayerAsync(steamId);
                
                if (steamPlayer != null)
                {
                    player = new Player(steamId, steamPlayer.personaname, countryCode);
                    player.PlayerAvatar = await httpClient.GetByteArrayAsync(steamPlayer.avatarfull);
                    player = database.CreatePlayer(player);
                }
            }

            List<Claim> claims = new List<Claim>();

            claims.Add(new Claim(ClaimTypes.Name, player.PlayerId));
            claims.Add(new Claim(ClaimTypes.Role, player.Role));

            context.Principal.AddIdentity(new ClaimsIdentity(claims, "DefaultAuth"));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseResponseCompression();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBlazorDebugging();
            }

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
