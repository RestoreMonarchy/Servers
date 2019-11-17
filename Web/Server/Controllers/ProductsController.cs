using Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestoreMonarchy.Database;
using System;
using System.Collections.Generic;
using Web.Server.Extensions.Database;

namespace Web.Server.Controllers
{
    [ApiController]
    [Authorize(Roles = "Moderator, Admin")]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IDatabaseManager _database;
        public ProductsController(IDatabaseManager database)
        {
            _database = database;
        }

        [HttpGet]
        [AllowAnonymous]
        public List<Product> GetProducts()
        {
            return _database.GetProducts();
        }

        [HttpPost]
        public Product CreateProduct([FromBody] Product product)
        {            
            product = _database.GetProduct(_database.CreateProduct(product));
            return product;
        }

        [HttpPatch]
        public Product PatchProduct([FromBody] Product product)
        {
            _database.UpdateProduct(product);
            return product;
        }

        [HttpPut("{productId}")]
        public bool ToggleProduct(int productId)
        {
            return _database.ToggleProduct(productId);
        }

        [HttpGet("{productId}")]
        [AllowAnonymous]
        public ActionResult<Product> GetProduct(short productId)
        {
            return _database.GetProduct(productId);
        }
    }
}