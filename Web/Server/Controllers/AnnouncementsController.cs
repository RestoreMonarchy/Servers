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
    public class AnnouncementsController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly Database database;

        private SqlConnection connection => new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

        public AnnouncementsController(IConfiguration configuration, Database database)
        {
            this.configuration = configuration;
            this.database = database;
        }

        [HttpPost]
        public ActionResult<int> AddAnnouncement([FromBody] Announcement announcement)
        {
            string sql = "INSERT INTO dbo.Announcements (Title, Content, AuthorId) VALUES (@Title, @Content, @AuthorId);";
            int rows = 0;
            using (var conn = connection)
            {
                rows = conn.Execute(sql, new { announcement.Title, announcement.Content, AuthorId = (long)announcement.AuthorId  });
            }

            return Ok(rows);
        }
        [HttpGet]
        public ActionResult<List<Announcement>> GetAnnouncements([FromHeader] int pages = 5)
        {
            string sql = "SELECT TOP(@pages) a.*, p.* FROM dbo.Announcements as a INNER JOIN dbo.Players as p ON a.AuthorId = p.PlayerId ORDER BY a.CreateDate DESC;";
            List<Announcement> announcements;
            using (var conn = connection)
            {
                announcements = conn.Query<Announcement, Player, Announcement>(sql, (a, p) => 
                {
                    a.Author = p;
                    return a;
                }, new { pages }, splitOn: "PlayerId").ToList();
            }
            return announcements;
        }

        [HttpGet("{announcementId}")]
        public ActionResult<Announcement> GetAnnouncement(int announcementId)
        {
            string sql = "SELECT a.*, p.* FROM dbo.Announcements as a INNER JOIN dbo.Players as p ON a.AuthorId = p.PlayerId WHERE a.AnnouncementId = @announcementId;";
            Announcement announcement;
            using (var conn = connection)
            {
                announcement = conn.Query<Announcement, Player, Announcement>(sql, (a, p) =>
                {
                    a.Author = p;
                    return a;
                }, new { announcementId }, splitOn: "PlayerId").FirstOrDefault();
            }
            return announcement;
        }

        [HttpPatch]
        public ActionResult<int> PatchAnnouncement([FromBody] Announcement announcement)
        {
            string sql = "UPDATE dbo.Announcements SET Title = @Title, Content = @Content, AuthorId = @AuthorId, UpdateDate = @UpdateDate WHERE AnnouncementId = @AnnouncementId;";
            int rows = 0;
            using (var conn = connection)
            {
                rows = conn.Execute(sql, new { announcement.Title, announcement.Content, AuthorId = (long)announcement.AuthorId, UpdateDate = DateTime.Now, announcement.AnnouncementId });
            }

            return Ok(rows);
        }

        [HttpDelete("{announcementId}")]
        public ActionResult<int> DeleteAnnouncement(int announcementId)
        {
            string sql = "DELETE FROM dbo.Announcements WHERE AnnouncementId = @announcementId;";
            int rows = 0;
            using (var conn = connection)
            {
                rows = conn.Execute(sql, new { announcementId });
            }

            return Ok(rows);
        }
    }
}
