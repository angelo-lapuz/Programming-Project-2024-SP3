using WebAPI.Models;
using Microsoft.EntityFrameworkCore;
using Peak = WebAPI.Models.Peak;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace WebAPI.Data
{
    public class PeakHubContext : IdentityDbContext<IdentityUser>
    {
        public PeakHubContext(DbContextOptions<PeakHubContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Peak> Tasks { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Board> Boards { get; set; }
        public DbSet<Award> Awards { get; set; }
    }
}
