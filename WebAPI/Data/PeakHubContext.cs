using WebAPI.Models;
using Microsoft.EntityFrameworkCore;
using Task = WebAPI.Models.Task;

namespace WebAPI.Data
{
    public class PeakHubContext : DbContext
    {
        public PeakHubContext(DbContextOptions<PeakHubContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Task> Tasks { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Board> Boards { get; set; }
        public DbSet<Award> Awards { get; set; }
    }
}
