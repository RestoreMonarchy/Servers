using Core.Models;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Web.Server.Utilities;

namespace Web.Server.Controllers
{
    [ApiController]
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
    }
}
