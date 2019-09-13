using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using RestoreMonarchy.Core;

namespace RestoreMonarchy.WebAPI.Controllers
{
    [ApiController]
    public class PermissionsController : ControllerBase
    {
        private readonly IConfiguration configuration;

        private SqlConnection connection => new SqlConnection(configuration["ConnectionString"]);

        public PermissionsController(IConfiguration configuration)
        {

        }


        [Route("api/[controller]/AddGroup")]
        [HttpPost]
        public ActionResult<int> AddGroup([FromBody] PermissionGroup group)
        {
            string sql = "INSERT INTO dbo.PermissionGroups (GroupID, GroupName, GroupColor, GroupPriority) VALUES (@GroupID, @GroupName, @GroupColor, @GroupPriority);";
            using (var conn = connection)
            {
                return conn.Execute(sql, new { group });
            }
        }

        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
