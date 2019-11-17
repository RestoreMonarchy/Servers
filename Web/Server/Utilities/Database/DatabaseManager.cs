using Core.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Data.SqlClient;
using System.Net.Http;
using System.Threading.Tasks;

namespace Web.Server.Utilities.Database
{
    public class DatabaseManager
    {
        public readonly IConfiguration configuration;
        public SqlConnection connection => new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        public FileDataManager KitsDataManager { get; private set; }
        public HttpClient HttpClient { get; set; }

        public delegate Task OnPlayerCreated(Player player);
        public event OnPlayerCreated onPlayerCreated;

        public DatabaseManager(IConfiguration configuration)
        {
            this.configuration = configuration;
            this.HttpClient = new HttpClient();
            KitsDataManager = new FileDataManager(configuration.GetValue<string>(WebHostDefaults.ContentRootKey), "kits.json");
        }

        public async Task InvokePlayerCreatedAsync(Player player)
        {
            await onPlayerCreated.Invoke(player);
        }
    }
}
