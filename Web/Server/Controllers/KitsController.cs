using AspNet.Security.ApiKey.Providers;
using Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestoreMonarchy.Database;
using RestoreMonarchy.Database.FileDatabase;
using System.Collections.Generic;

namespace Web.Server.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = ApiKeyDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    public class KitsController : ControllerBase
    {
        public List<Kit> Kits { get; set; }
        private readonly IFileDatabase _kitsDatabase;

        public KitsController(IDatabaseManager database)
        {
            this._kitsDatabase = database.GetFileDatabase("kits");
            Kits = database.GetFileDatabase("kits").ReadObject<Kit>();
        }

        [HttpGet]        
        public ActionResult<List<Kit>> GetKits([FromHeader] string apikey)
        {
            Kits = _kitsDatabase.ReadObject<Kit>();
            return Ok(Kits);
        }

        [HttpPost]
        public void PostKit([FromBody] Kit kit)
        {
            Kits.Add(kit);
            _kitsDatabase.SaveObject(Kits);
        }

        [HttpDelete("{kitName}")]
        public void DeleteKit(string kitName)
        {
            Kits.RemoveAll(x => x.Name == kitName);
            _kitsDatabase.SaveObject(Kits);
        }
    }
}
