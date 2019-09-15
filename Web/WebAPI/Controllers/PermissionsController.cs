using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Core;

namespace RestoreMonarchy.WebAPI.Controllers
{
    [ApiController]
    public class PermissionsController : ControllerBase
    {
        private readonly IConfiguration configuration;

        private SqlConnection connection => new SqlConnection(configuration["ConnectionString"]);

        public PermissionsController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        [HttpPost("api/Permissions/Members")]
        public ActionResult<int> AddPlayerToGroup([FromBody] PermissionGroup.PermissionMember member)
        {
            Console.WriteLine(member.GroupID + " " + member.SteamID);
            string sql = "INSERT INTO dbo.PermissionMembers (GroupID, SteamID) VALUES (@GroupID, @SteamID);";
            try
            {
                using (var conn = connection)
                {
                    return Ok(conn.Execute(sql, new { member.GroupID, SteamID = (long)member.SteamID }));
                }
            } catch
            {
                return NotFound();
            }
            
        }

        [HttpDelete("api/Permissions/Members")]
        public ActionResult<int> RemovePlayerFromGroup([FromBody] PermissionGroup.PermissionMember member)
        {
            string sql = "DELETE FROM dbo.PermissionMembers WHERE SteamID = @SteamID AND GroupID = @GroupID;";
            using (var conn = connection)
            {
                return Ok(conn.Execute(sql, new { SteamID = (long)member.SteamID, member.GroupID }));
            }
        }

        [HttpPost("api/Permissions/Groups")]
        public ActionResult<int> AddGroup([FromBody] PermissionGroup group)
        {
            string sql = "INSERT INTO dbo.PermissionGroups (GroupID, GroupName, GroupColor, GroupPriority) VALUES (@GroupID, @GroupName, @GroupColor, @GroupPriority);";

            using (var conn = connection)
            {
                return Ok(conn.Execute(sql, new { group.GroupID, group.GroupName, group.GroupColor, group.GroupPriority }));
            }
            
        }

        [HttpDelete("api/Permissions/Groups/{groupID}")]
        public ActionResult<int> DeleteGroup(string groupID)
        {
            string sql = "DELETE FROM dbo.PermissionGroups WHERE GroupID = @GroupID;";
            using (var conn = connection)
            {
                return Ok(conn.Execute(sql, new { GroupID = groupID }));
            }
        }

        [HttpGet("api/Permissions/Groups/{id}")]
        public ActionResult<PermissionGroup> GetGroup([FromRoute] string id)
        {
            string sql = "SELECT g.*, p.*, m.* FROM dbo.PermissionGroups AS g LEFT JOIN dbo.Permissions as p ON p.GroupID = g.GroupID " +
                "LEFT JOIN dbo.PermissionMembers AS m ON m.GroupID = g.GroupID WHERE g.GroupID = @GroupID;";
            PermissionGroup group = null;

            using (var conn = connection)
            {
                conn.Query<PermissionGroup, PermissionGroup.Permission, PermissionGroup.PermissionMember, PermissionGroup>(sql,
                        (g, p, m) =>
                        {
                            if (group == null)
                                group = g;

                            if (group.Permissions == null)
                                group.Permissions = new List<PermissionGroup.Permission>();
                            if (group.Members == null)
                                group.Members = new List<PermissionGroup.PermissionMember>();

                            if (p != null && !group.Permissions.Exists(x => x.PermissionID == p.PermissionID))
                                group.Permissions.Add(p);
                            if (m != null && !group.Members.Exists(x => m.SteamID == m.SteamID))
                                group.Members.Add(m);

                            return g;
                        }, splitOn: "GroupID,GroupID", param: new { GroupID = id });
            }

            return Ok(group);
        }

        [HttpGet("api/Permissions/Members/{steamID}")]
        public ActionResult<List<PermissionGroup>> GetGroups(ulong steamID)
        {
            string sql = "SELECT m.*, g.*, p.* FROM dbo.PermissionMembers AS m LEFT JOIN dbo.PermissionGroups as g ON g.GroupID = m.GroupID " +
                "LEFT JOIN dbo.Permissions as p ON p.GroupID = m.GroupID WHERE m.SteamID = @SteamID;";
            string sql2 = "SELECT g.*, p.* FROM dbo.PermissionGroups AS g LEFT JOIN dbo.Permissions as p ON p.GroupID = g.GroupID WHERE g.GroupID = 'default';";

            List<PermissionGroup> groups = new List<PermissionGroup>();

            using (var conn = connection)
            {
                conn.Query<PermissionGroup.PermissionMember, PermissionGroup, PermissionGroup.Permission, PermissionGroup.PermissionMember>(sql,
                    (m, g, p) =>
                    {
                        PermissionGroup group = groups.FirstOrDefault(x => x.GroupID == g.GroupID);

                        if (group == null)
                        {
                            group = g;
                            groups.Add(group);
                        }

                        if (group.Permissions == null)
                            group.Permissions = new List<PermissionGroup.Permission>();
                        if (group.Members == null)
                            group.Members = new List<PermissionGroup.PermissionMember>();

                        if (p != null && !group.Permissions.Exists(x => x.PermissionID == p.PermissionID))
                            group.Permissions.Add(p);
                        if (m != null && !group.Members.Exists(x => m.SteamID == m.SteamID))
                            group.Members.Add(m);

                        return m;
                    }, splitOn: "GroupID,GroupID", param: new { SteamID = (long)steamID });

                conn.Query<PermissionGroup, PermissionGroup.Permission, PermissionGroup>(sql2, 
                    (g, p) => 
                    {
                        PermissionGroup group = groups.FirstOrDefault(x => x.GroupID == g.GroupID);

                        if (group == null)
                        {
                            group = g;
                            groups.Add(group);
                        }

                        if (group.Permissions == null)
                            group.Permissions = new List<PermissionGroup.Permission>();
                        if (group.Members == null)
                            group.Members = new List<PermissionGroup.PermissionMember>();

                        if (p != null && !group.Permissions.Exists(x => x.PermissionID == p.PermissionID))
                            group.Permissions.Add(p);

                        return g;
                    }, splitOn: "GroupID");
            }

            return Ok(groups);
        }
    }
}
