using Core.Models;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Web.Server.Utilities;

namespace Web.Server.Controllers
{
    [ApiController]
    [Authorize(Roles = "Moderator, Admin")]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly Database database;
        
        private SqlConnection connection => new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

        public ProductsController(IConfiguration configuration, Database database)
        {
            this.configuration = configuration;
            this.database = database;
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult<List<Product>> GetProducts([FromHeader] string category)
        {
            string sql = "SELECT * FROM dbo.Products";

            if (category != null)
                sql = sql + " WHERE Category = @category";

            List<Product> products;

            using (var conn = connection)
            {
                products = conn.Query<Product>(sql, new { category }).ToList();
            }

            return Ok(products);
        }

        [HttpPost]
        public ActionResult<int> PostProduct([FromBody] Product product)
        {
            string sql = "INSERT INTO dbo.Products (Name, Description, Category, Price, Duration) VALUES (@Name, @Description, @Category, @Price, @Duration);";
            int rows;
            using (var conn = connection)
            {
                rows = conn.Execute(sql, product);
            }

            return Ok(rows);
        }

        [HttpPatch]
        public ActionResult<int> PatchProduct([FromBody] Product product)
        {
            string sql = "UPDATE dbo.Products SET Name = @Name, Description = @Description, Category = @Category, Price = @Price, Duration = @Duration WHERE ProductId = @ProductId;";
            int rows;
            using (var conn = connection)
            {
                rows = conn.Execute(sql, product);
            }

            return Ok(rows);
        }

        [HttpDelete("productId")]
        public ActionResult<int> DeleteProduct(string productId)
        {
            string sql = "DELETE FROM dbo.Products WHERE ProductId = @productId;";
            int rows;
            using (var conn = connection)
            {
                rows = conn.Execute(sql, new { productId });
            }

            return Ok(rows);
        }
    }
}
