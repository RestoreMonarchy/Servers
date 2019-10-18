using Core.Models;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Web.Server.Utilities;
using Web.Server.Utilities.Database;

namespace Web.Server.Controllers
{
    [ApiController]
    [Authorize(Roles = "Moderator, Admin")]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly DatabaseManager database;
        public ProductsController(DatabaseManager database)
        {
            this.database = database;
        }

        [HttpGet]
        [AllowAnonymous]
        public List<Product> GetProducts([FromHeader] string category)
        {
            return database.GetProducts();
        }

        [HttpPost]
        public Product CreateProduct([FromBody] Product product)
        {
            product.LastUpdate = DateTime.Now;
            product.CreateDate = DateTime.Now;

            product.ProductId = database.CreateProduct(product);
            return product;
        }

        [HttpPatch]
        public Product PatchProduct([FromBody] Product product)
        {
            product.LastUpdate = DateTime.Now;

            database.UpdateProduct(product);
            return product;
        }

        [HttpPut("{productId}")]
        public bool ToggleProduct(int productId)
        {
            return database.ToggleProduct(productId);
        }

        [HttpGet("{productId}")]
        public ActionResult<Product> GetProduct(int productId)
        {
            return database.GetProduct(productId);
        }
    }
}
