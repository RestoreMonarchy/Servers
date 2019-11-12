using AspNet.Security.ApiKey.Providers;
using Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Web.Server.Utilities.Database;

namespace Web.Server.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = ApiKeyDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    public class KitsController : ControllerBase
    {
        public List<Kit> Kits { get; set; }
        private readonly DatabaseManager database;

        public KitsController(DatabaseManager database)
        {
            this.database = database;
            Kits = database.KitsDataManager.ReadObject<Kit>();
        }

        [HttpGet]        
        public ActionResult<List<Kit>> GetKits([FromHeader] string apikey)
        {
            Kits = database.KitsDataManager.ReadObject<Kit>();
            return Ok(Kits);
        }

        [HttpPost]
        public void PostKit([FromBody] Kit kit)
        {
            Kits.Add(kit);
            database.KitsDataManager.SaveObject(Kits);
        }

        [HttpDelete("{kitName}")]
        public void DeleteKit(string kitName)
        {
            Kits.RemoveAll(x => x.Name == kitName);
            database.KitsDataManager.SaveObject(Kits);
        }
    }
}
