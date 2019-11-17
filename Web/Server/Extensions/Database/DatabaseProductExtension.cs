using Core.Models;
using Dapper;
using RestoreMonarchy.Database;
using System.Collections.Generic;
using System.Linq;

namespace Web.Server.Extensions.Database
{
    public static class DatabaseProductExtension
    {
        public static List<Product> GetProducts(this IDatabaseManager database)
        {
            string sql = "SELECT p.*, r.* FROM dbo.Products AS p LEFT JOIN dbo.Ranks AS r ON p.RankId = r.RankId;";

            using (var conn = database.Connection)
            {
                return conn.Query<Product, Rank, Product>(sql, (p, r) =>
                {
                    p.Rank = r;
                    return p;
                }, splitOn: "RankId").ToList();
            }
        }

        public static short CreateProduct(this IDatabaseManager database, Product product)
        {
            string sql = "INSERT INTO dbo.Products (ShortName, Name, Description, Price, RankId, Coins) OUTPUT INSERTED.ProductId " +
                "VALUES (@ShortName, @Name, @Description, @Price, @RankId, @Coins);";

            using (var conn = database.Connection)
            {
                return conn.ExecuteScalar<short>(sql, product);
            }
        }

        public static void UpdateProduct(this IDatabaseManager database, Product product)
        {
            string sql = "UPDATE dbo.Products SET ShortName = @ShortName, Name = @Name, Description = @Description, Price = @Price WHERE ProductId = @ProductId;";

            using (var conn = database.Connection)
            {
                conn.Execute(sql, product);
            }
        }

        public static bool ToggleProduct(this IDatabaseManager database, int productId)
        {
            string sql = "UPDATE dbo.Products SET ActiveFlag = ~ActiveFlag OUTPUT INSERTED.ActiveFlag WHERE ProductId = @ProductId;";

            using (var conn = database.Connection)
            {
                return conn.ExecuteScalar<bool>(sql, new { productId });
            }
        }

        public static Product GetProduct(this IDatabaseManager database, short productId)
        {
            string sql = "SELECT p.*, r.* FROM dbo.Products AS p LEFT JOIN dbo.Ranks AS r ON p.RankId = r.RankId WHERE p.ProductId = @productId;";

            using (var conn = database.Connection)
            {
                return conn.Query<Product, Rank, Product>(sql, (p, r) => 
                {
                    p.Rank = r;
                    return p;
                }, new { productId }, splitOn: "RankId").FirstOrDefault();
            }
        }
    }
}
