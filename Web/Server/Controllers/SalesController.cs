using Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Server.Utilities.Database;

namespace Web.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SalesController : ControllerBase
    {
        private readonly DatabaseManager _database;
        public SalesController(DatabaseManager database)
        {
            _database = database;
        }

        [HttpPost]
        public int PostSale(Sale sale)
        {
            var product = _database.GetProduct(sale.ProductId);
            sale.Price = product.Price;
            sale.Currency = "EUR";
            return _database.CreateSale(sale);
        }
    }
}
