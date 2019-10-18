using Core.Models;
using Dapper;
using System.Collections.Generic;
using System.Linq;

namespace Web.Server.Utilities.Database
{
    public static class DatabaseProductExtension
    {
        public static List<Product> GetProducts(this DatabaseManager database)
        {
            string sql = "SELECT * FROM dbo.Products;";

            using (var conn = database.connection)
            {
                return conn.Query<Product>(sql).ToList();
            }
        }

        public static int CreateProduct(this DatabaseManager database, Product product)
        {
            string sql = "INSERT INTO dbo.Products (Label, Details, Picture, Price, Category, Duration, LastUpdate, CreateDate) OUTPUT INSERTED.ProductId " +
                "VALUES (@Label, @Details, @Picture, @Price, @Category, @Duration, @LastUpdate, @CreateDate);";

            using (var conn = database.connection)
            {
                return conn.ExecuteScalar<int>(sql, product);
            }
        }

        public static void UpdateProduct(this DatabaseManager database, Product product)
        {
            string sql = "UPDATE dbo.Products SET Label = @Label, Details = @Details, Picture = @Picture, Price = @Price, Category = @Category, " +
                "LastUpdate = @LastUpdate, Duration = @Duration WHERE ProductId = @ProductId;";

            using (var conn = database.connection)
            {
                conn.Execute(sql, product);
            }
        }

        public static bool ToggleProduct(this DatabaseManager database, int productId)
        {
            string sql = "UPDATE dbo.Tickets SET Status = ~Status OUTPUT INSERTED.Status WHERE TicketId = @ticketId;";

            using (var conn = database.connection)
            {
                return conn.ExecuteScalar<bool>(sql, new { productId });
            }
        }

        public static Product GetProduct(this DatabaseManager database, int productId)
        {
            string sql = "SELECT * FROM  dbo.Products WHERE ProductId = @productId;";

            using (var conn = database.connection)
            {
                return conn.QuerySingle(sql, new { productId });
            }
        }
    }
}
