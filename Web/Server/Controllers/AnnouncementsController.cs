using Core.Models;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Claims;

namespace Web.Server.Controllers
{
    [ApiController]
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    public class AnnouncementsController : ControllerBase
    {
        private readonly IConfiguration configuration;

        private SqlConnection connection => new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

        public AnnouncementsController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        [HttpPost]
        public ActionResult<Announcement> AddAnnouncement([FromBody] Announcement announcement)
        {
            string sql = "INSERT INTO dbo.Announcements (Title, Content, AuthorId, LastUpdate, CreateDate) " +
                "VALUES (@Title, @Content, @AuthorId, @LastUpdate, @CreateDate);";

            announcement.CreateDate = DateTime.Now;
            announcement.LastUpdate = DateTime.Now;
            announcement.AuthorId = User.FindFirst(x => x.Type == ClaimTypes.Name).Value;

            using (var conn = connection)
            {
                announcement.AnnouncementId = conn.ExecuteScalar<int>(sql, announcement);
            }

            return Ok(announcement);
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult<List<Announcement>> GetAnnouncements([FromQuery] int pages = 5)
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

            return Ok(announcements);
        }

        [HttpGet("{announcementId}")]
        [AllowAnonymous]
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
            return Ok(announcement);
        }

        [HttpPut]
        public ActionResult<int> PutAnnouncement([FromBody] Announcement announcement)
        {
            string sql = "UPDATE dbo.Announcements SET Title = @Title, Content = @Content, AuthorId = @AuthorId, LastUpdate = @LastUpdate WHERE AnnouncementId = @AnnouncementId;";
            int rows = 0;
            using (var conn = connection)
            {
                rows = conn.Execute(sql, new { announcement.Title, announcement.Content, AuthorId = announcement.AuthorId, LastUpdate = DateTime.Now, announcement.AnnouncementId });
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
