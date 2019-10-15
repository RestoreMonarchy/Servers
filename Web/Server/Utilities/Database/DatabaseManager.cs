using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;

namespace Web.Server.Utilities.Database
{
    public class DatabaseManager
    {
        private readonly IConfiguration configuration;
        public SqlConnection connection => new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

        public DatabaseManager(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
    }
}
